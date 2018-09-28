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
    public class TeamController : Controller
    {
        [HttpGet]
        [Route("GetTeams")]
        public List<Team> GetTeams()
        {
            string query = "SELECT * FROM TeamTable;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.Query<Team>(query).ToList();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return new List<Team>();
                }
            }
        }

        [HttpGet]
        [Route("GetTeamSites")]
        public List<TeamSite> GetTeamSites([FromQuery]int id, [FromQuery]string startdate, [FromQuery]string enddate)
        {
            Team team = GetByIdStatic(id);
            List<TeamSite> teamsites = new List<TeamSite>();
            List<string> dates = new List<string>();
            for (DateTime dt = DateTime.Parse(startdate); dt <= DateTime.Parse(enddate); dt = dt.AddDays(1))
            {
                dates.Add(dt.ToString("yyyy-MM-dd"));
            }
            foreach(string date in dates)
            {
                TeamSite temp = new TeamSite()
                {
                    Date = date,
                    Day = DateTime.Parse(date).DayOfWeek.ToString(),
                    SV1Name = team.SV1Name,
                    SV1Site = SessionController.GetSite(date, team.SV1Id),
                    DRI1Name = team.DRI1Name,
                    DRI1Site = SessionController.GetSite(date, team.DRI1Id),
                    DRI2Name = team.DRI2Name,
                    DRI2Site = SessionController.GetSite(date, team.DRI2Id),
                    RN1Name = team.RN1Name,
                    RN1Site = SessionController.GetSite(date, team.RN1Id),
                    RN2Name = team.RN2Name,
                    RN2Site = SessionController.GetSite(date, team.RN2Id),
                    RN3Name = team.RN3Name,
                    RN3Site = SessionController.GetSite(date, team.RN3Id),
                    CCA1Name = team.CCA1Name,
                    CCA1Site = SessionController.GetSite(date, team.CCA1Id),
                    CCA2Name = team.CCA2Name,
                    CCA2Site = SessionController.GetSite(date, team.CCA2Id),
                    CCA3Name = team.CCA3Name,
                    CCA3Site = SessionController.GetSite(date, team.CCA3Id)
                };
                teamsites.Add(temp);
            }
            return teamsites;
        }

        [HttpPost]
        [Route("Create")]
        public int Create()
        {
            Team team = new Team();
            using (StreamReader sr = new StreamReader(Request.Body))
            {
                team = JsonConvert.DeserializeObject<Team>(sr.ReadToEnd());
            }
            if (team != null)
            {
                string query = "IF NOT EXISTS (SELECT * FROM TeamTable WHERE Name=@Name) " +
                 "INSERT INTO TeamTable (Name, SV1Id, SV1Name, DRI1Id, DRI1Name, DRI2Id, DRI2Name, RN1Id, RN1Name, " +
                "RN2Id, RN2Name, RN3Id, RN3Name, CCA1Id, CCA1Name, CCA2Id, CCA2Name, CCA3Id, CCA3Name) " +
                "VALUES (@Name, @SV1Id, @SV1Name, @DRI1Id, @DRI1Name, @DRI2Id, @DRI2Name, @RN1Id, @RN1Name, " +
                "@RN2Id, @RN2Name, @RN3Id, @RN3Name, @CCA1Id, @CCA1Name, @CCA2Id, @CCA2Name, @CCA3Id, @CCA3Name);";
                using (SqlConnection conn = new SqlConnection(Connection.ConnString))
                {
                    try
                    {
                        conn.Open();
                        return conn.Execute(query, team);
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

        [HttpGet]
        [Route("GetById")]
        public Team GetById([FromQuery]int id)
        {
            return GetByIdStatic(id);
        }

        public static Team GetByIdStatic(int id)
        {
            string query = "SELECT * FROM TeamTable WHERE Id=@Id;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.QueryFirstOrDefault<Team>(query, new { id });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return null;
                }
            }
        }
        
        [HttpGet]
        [Route("GetTeamName")]
        [Produces("application/json")]
        public string GetTeamName([FromQuery]int id)
        {
            string query = "SELECT Name FROM TeamTable WHERE Id=@Id;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.QueryFirstOrDefault<string>(query, new { id });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return null;
                }
            }
        }

        [HttpPut]
        [Route("Update")]
        public int Update()
        {
            Team employee = new Team();
            using (StreamReader sr = new StreamReader(Request.Body))
            {
                employee = JsonConvert.DeserializeObject<Team>(sr.ReadToEnd());
            }
            if (employee != null)
            {
                string query = "UPDATE TeamTable " +
                "SET SV1Id=@SV1Id, SV1Name=@SV1Name, DRI1Id=@DRI1Id, DRI1Name=@DRI1Name, " +
                "DRI2Id=@DRI2Id, DRI2Name=@DRI2Name, RN1Id=@RN1Id, RN1Name=@RN1Name, " +
                "RN2Id=@RN2Id, RN2Name=@RN2Name, RN3Id=@RN3Id, RN3Name=@RN3Name, " +
                "CCA1Id=@CCA1Id, CCA1Name=@CCA1Name, CCA2Id=@CCA2Id, CCA2Name=@CCA2Name, " +
                "CCA3Id=@CCA3Id, CCA3Name=@CCA3Name WHERE Name=@Name;";
                using (SqlConnection conn = new SqlConnection(Connection.ConnString))
                {
                    try
                    {
                        conn.Open();
                        return conn.Execute(query, employee);
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
                string query = "DELETE FROM TeamTable WHERE Id=@Id;";
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

        private string GetTeamSite(string date, int id)
        {
            string site = SessionController.GetSite(date, id);
            if (site == "")
            {
                site = AbsenceController.GetStaffStatus(id, date);
                site = "";
            }
            return site;
        }
    }
}
