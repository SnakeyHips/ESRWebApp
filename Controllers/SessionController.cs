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
            string queryList = "SELECT * From SessionEmployeeTable WHERE SessionId=@Id";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    List<Session> temp =  conn.Query<Session>(query, new { date }).ToList();
                    foreach(Session s in temp)
                    {
                        s.Employees = conn.Query<SessionEmployee>(queryList, new { s.Id }).ToList();
                    }
                    return temp;
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
            string queryList = "SELECT * FROM SessionEmployeeTable WHERE SessionId=@Id";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    Session temp = conn.QueryFirstOrDefault<Session>(query, new { id });
                    temp.Employees = conn.Query<SessionEmployee>(queryList, new { id }).ToList();
                    return temp;
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
                "INSERT INTO SessionTable (Date, Day, Type, Site, Time, LOD, Chairs, OCC, Estimate, Holiday, Note, StaffCount, State) " +
                "VALUES (@Date, @Day, @Type, @Site, @Time, @LOD, @Chairs, @OCC, @Estimate, @Holiday, @Note, @StaffCount, @State);";
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
