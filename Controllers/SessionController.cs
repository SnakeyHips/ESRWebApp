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

        [HttpGet]
        [Route("GetSessions")]
        public List<Session> GetSessions()
        {
            string query = "SELECT * FROM SessionTable;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return new List<Session>(conn.Query<Session>(query).ToList());
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return new List<Session>();
                }
            }
        }

        [HttpGet("{id}")]
        [Route("GetById")]
        public Session GetById(int id)
        {
            string query = "SELECT * FROM SessionTable WHERE Id=@Id;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.QuerySingle<Session>(query, new { id });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return null;
                }
            }
        }

        [HttpGet("{date}/{staffid}")]
        [Route("GetStaff")]
        public Session GetStaff(string date, int staffid)
        {
            string query = "SELECT * FROM SessionTable WHERE Date=@Date AND @StaffId IN" +
                "(SV1Id, DRI1Id, DRI2Id, RN1Id, RN2Id, RN3Id, CCA1Id, CCA2Id, CCA3Id)";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.QuerySingle<Session>(query, new { date, staffid });
                }
                catch
                {
                    return null;
                }
            }
        }

        [HttpGet("{date}/{staffid}")]
        [Route("GetSite")]
        public string GetSite(string date, int staffid)
        {
            string query = "SELECT Site FROM SessionTable WHERE Date=@Date AND @StaffId IN" +
                "(SV1Id, DRI1Id, DRI2Id, RN1Id, RN2Id, RN3Id, CCA1Id, CCA2Id, CCA3Id)";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.QuerySingle<string>(query, new { date, staffid });
                }
                catch
                {
                    return "";
                }
            }
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
                string query = "UPDATE SessionTable" +
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
                    "SET SV1Id=@SV1Id, SV1Name=@SV1Name, SV1LOD=@SV1LOD, SV1UNS=@SV1UNS, SV1OT=@SV1OT, " +
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

        [HttpDelete("{id}")]
        [Route("Delete")]
        public int Delete(int id)
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
            //else
            //{
            //    foreach (Holiday h in AdminViewModel.Holidays)
            //    {
            //        if (h.Date.Equals(date))
            //        {
            //            holiday = 2;
            //        }
            //    }
            //}
            return holiday;
        }
    }
}
