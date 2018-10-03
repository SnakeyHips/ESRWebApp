using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ERSWebApp.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Newtonsoft.Json;

namespace ERSWebApp.Controllers
{
    [Route("api/[controller]")]
    public class SessionController : Controller
    {

        [HttpGet()]
        [Route("GetSessions")]
        public List<Session> GetSessions([FromQuery]string date)
        {
            string query = "SELECT * FROM SessionTable WHERE Date=@Date;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.Query<Session>(query, new { date }).ToList();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return new List<Session>();
                }
            }
        }

        [HttpGet()]
        [Route("GetById")]
        public Session GetById([FromQuery]int id)
        {
            string query = "SELECT * FROM SessionTable WHERE Id=@Id;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.QueryFirstOrDefault<Session>(query, new { id });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return null;
                }
            }
        }

        [HttpGet()]
        [Route("GetStaff")]
        public Session GetStaff([FromQuery]string date, [FromQuery]int staffid)
        {
            string query = "SELECT * FROM SessionTable WHERE Date=@Date AND @StaffId IN" +
                "(SV1Id, DRI1Id, DRI2Id, RN1Id, RN2Id, RN3Id, CCA1Id, CCA2Id, CCA3Id)";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.QueryFirst<Session>(query, new { date, staffid });
                }
                catch
                {
                    return null;
                }
            }
        }

        public static string GetSite(string date, int staffid)
        {
            string site = "";
            string query = "SELECT Site FROM SessionTable WHERE Date=@Date AND @StaffId IN" +
                "(SV1Id, DRI1Id, DRI2Id, RN1Id, RN2Id, RN3Id, CCA1Id, CCA2Id, CCA3Id)";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    site = conn.QueryFirstOrDefault<string>(query, new { date, staffid });
                    if(site == null)
                    {
                        site = AbsenceController.GetStaffStatus(staffid, date);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }
            return site;
        }

        [HttpPost]
        [Route("Create")]
        public int Create()
        {
            Session session = new Session();
            using (StreamReader sr = new StreamReader(Request.Body))
            {
                session = JsonConvert.DeserializeObject<Session>(sr.ReadToEnd());
                session.Day = DateTime.Parse(session.Date).DayOfWeek.ToString();
                session.Holiday = CheckHoliday(session.Date, session.Day);
            }
            if (session != null)
            {
                string query = "IF NOT EXISTS (SELECT * FROM SessionTable WHERE Date=@Date AND Site=@Site AND Time=@Time) " +
                "INSERT INTO SessionTable (Date, Day, Type, Site, Time, LOD, Chairs, OCC, Estimate, Holiday, Note, SV1Id, SV1Name, " +
                "SV1LOD, SV1UNS, SV1OT, DRI1Id, DRI1Name, DRI1LOD, DRI1UNS, DRI1OT, DRI2Id, DRI2Name, DRI2LOD, DRI2UNS, DRI2OT, RN1Id, RN1Name, " +
                "RN1LOD, RN1UNS, RN1OT, RN2Id, RN2Name, RN2LOD, RN2UNS, RN2OT, RN3Id, RN3Name, RN3LOD, RN3UNS, RN3OT, CCA1Id, CCA1Name, CCA1LOD, " +
                "CCA1UNS, CCA1OT, CCA2Id, CCA2Name, CCA2LOD, CCA2UNS, CCA2OT, CCA3Id, CCA3Name, CCA3LOD, CCA3UNS, CCA3OT, StaffCount, State) " +
                "VALUES (@Date, @Day, @Type, @Site, @Time, @LOD, @Chairs, @OCC, @Estimate, @Holiday, @Note, @SV1Id, @SV1Name, @SV1LOD, " +
                " @SV1UNS, @SV1OT, @DRI1Id, @DRI1Name, @DRI1LOD, @DRI1UNS, @DRI1OT, @DRI2Id, @DRI2Name, @DRI2LOD, @DRI2UNS, @DRI1OT, @RN1Id, @RN1Name, " +
                "@RN1LOD, @RN1UNS, @RN1OT, @RN2Id, @RN2Name, @RN2LOD, @RN2UNS, @RN2OT, @RN3Id, @RN3Name, @RN3LOD, @RN3UNS, @RN3OT, @CCA1Id, @CCA1Name, " +
                "@CCA1LOD, @CCA1UNS, @CCA1OT, @CCA2Id, @CCA2Name, @CCA2LOD, @CCA2UNS, @CCA2OT, @CCA3Id, @CCA3Name, @CCA3LOD, @CCA3UNS, @CCA3OT, " +
                "@StaffCount, @State);";
                using (SqlConnection conn = new SqlConnection(Connection.ConnString))
                {
                    try
                    {
                        conn.Open();
                        return conn.Execute(query, session);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                        return -1;
                    }
                }
            }
            else
            {
                return -1;
            }
        }

        [HttpPut]
        [Route("Update")]
        public int Update()
        {
            Session session = new Session();
            using (StreamReader sr = new StreamReader(Request.Body))
            {
                session = JsonConvert.DeserializeObject<Session>(sr.ReadToEnd());
            }
            if (session != null)
            {
                string query = " UPDATE SessionTable" +
                    " SET Time=@Time, Type=@Type, Site=@Site, LOD=@LOD, Chairs=@Chairs, OCC=@OCC, Estimate=@Estimate" +
                    " WHERE Id=@Id;";
                using (SqlConnection conn = new SqlConnection(Connection.ConnString))
                {
                    try
                    {
                        conn.Open();
                        return conn.Execute(query, session);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                        return -1;
                    }
                }
            }
            else
            {
                return -1;
            }
        }

        public static int UpdateStaff(Session session)
        {
            if (session != null)
            {
                string query = "UPDATE SessionTable " +
                    "SET LOD=@LOD, OCC=@OCC, Estimate=@Estimate, " +
                    "SV1Id=@SV1Id, SV1Name=@SV1Name, SV1LOD=@SV1LOD, SV1UNS=@SV1UNS, SV1OT=@SV1OT, " +
                    "DRI1Id=@DRI1Id, DRI1Name=@DRI1Name, DRI1LOD=@DRI1LOD, DRI1UNS=@DRI1UNS, DRI1OT=@DRI1OT, " +
                    "DRI2Id=@DRI2Id, DRI2Name=@DRI2Name, DRI2LOD=@DRI2LOD, DRI2UNS=@DRI2UNS, DRI2OT=@DRI2OT, " +
                    "RN1Id=@RN1Id, RN1Name=@RN1Name, RN1LOD=@RN1LOD, RN1UNS=@RN1UNS, RN1OT=@RN1OT, " +
                    "RN2Id=@RN2Id, RN2Name=@RN2Name, RN2LOD=@RN2LOD, RN2UNS=@RN2UNS, RN2OT=@RN2OT, " +
                    "RN3Id=@RN3Id, RN3Name=@RN3Name, RN3LOD=@RN3LOD, RN3UNS=@RN3UNS, RN3OT=@RN3OT, " +
                    "CCA1Id=@CCA1Id, CCA1Name=@CCA1Name, CCA1LOD=@CCA1LOD, CCA1UNS=@CCA1UNS, CCA1OT=@CCA1OT, " +
                    "CCA2Id=@CCA2Id, CCA2Name=@CCA2Name, CCA2LOD=@CCA2LOD, CCA2UNS=@CCA2UNS, CCA2OT=@CCA2OT, " +
                    "CCA3Id=@CCA3Id, CCA3Name=@CCA3Name, CCA3LOD=@CCA3LOD, CCA3UNS=@CCA3UNS, CCA3OT=@CCA3OT, " +
                    "StaffCount=@StaffCount, State=@State, Note=@Note " +
                    "WHERE Id=@Id;";
                using (SqlConnection conn = new SqlConnection(Connection.ConnString))
                {
                    try
                    {
                        conn.Open();
                        return conn.Execute(query, session);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                        return -1;
                    }
                }
            }
            else
            {
                return -1;
            }
        }

        [HttpDelete]
        [Route("Delete")]
        public int Delete([FromQuery]int id)
        {
            if (id > 0)
            {
                string query = "DELETE FROM SessionTable WHERE Id=@Id;";
                using (SqlConnection conn = new SqlConnection(Connection.ConnString))
                {
                    try
                    {
                        conn.Open();
                        return conn.Execute(query, new { id });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                        return -1;
                    }
                }
            }
            else
            {
                return -1;
            }
        }

        [HttpGet()]
        [Route("GetEmployeeSessions")]
        public List<Session> GetEmployeeSessions([FromQuery]int staffid, [FromQuery]string startdate, [FromQuery]string enddate)
        {
            string query = "SELECT * FROM SessionTable WHERE Date BETWEEN @StartDate AND @EndDate AND @StaffId IN" +
                "(SV1Id, DRI1Id, DRI2Id, RN1Id, RN2Id, RN3Id, CCA1Id, CCA2Id, CCA3Id)";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.Query<Session>(query, new { startdate, enddate, staffid }).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        [HttpGet()]
        [Route("GetSites")]
        public List<Site> GetSites([FromQuery]string type)
        {
            string query = "SELECT * FROM SiteTable WHERE Type=@Type;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.Query<Site>(query, new { type }).ToList();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return new List<Site>();
                }
            }
        }

        //Method for checking if selected date is weekend/holiday
        public int CheckHoliday(string date, string day)
        {
            int holiday = 0;
            if (day.Equals("Saturday"))
            {
                holiday = 1;
            }
            else if (day.Equals("Sunday"))
            {
                holiday = 2;
            }
            else
            {
                foreach (SpecialDate sd in AdminController.GetSpecialDatesStatic())
                {
                    if (sd.Date.Equals(date))
                    {
                        holiday = 2;
                    }
                }
            }
            return holiday;
        }

        //Method for sites csv to table
        //public static int AddSites()
        //{
        //    List<Site> sites = new List<Site>();
        //    StreamReader file = new StreamReader("C:\\Source\\ERSWebApp\\ESRWebApp\\Sites.csv");
        //    string line;
        //    while ((line = file.ReadLine()) != null)
        //    {
        //        string[] array = line.Split(',');
        //        sites.Add(new Site()
        //        {
        //            Name = array[0],
        //            Type = array[1],
        //            Times = array[2]
        //        });
        //    }
        //    string query = "INSERT INTO SiteTable (Name, Type, Times)" +
        //            "VALUES (@Name, @Type, @Times);";
        //    using (SqlConnection conn = new SqlConnection(Connection.ConnString))
        //    {
        //        try
        //        {
        //            conn.Open();
        //            return conn.Execute(query, sites);
        //        }
        //        catch (Exception ex)
        //        {
        //            return -1;
        //        }
        //    }
        //}
    }
}
