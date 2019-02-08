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
    public class AdminController : Controller
    {
        [HttpGet]
        [Route("GetSpecialDates")]
        public List<SpecialDate> GetSpecialDates()
        {
            return GetSpecialDatesStatic();
        }

        public static List<SpecialDate> GetSpecialDatesStatic()
        {
            string query = "SELECT * FROM SpecialDateTable;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.Query<SpecialDate>(query).ToList();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return new List<SpecialDate>();
                }
            }
        }

        [HttpGet]
        [Route("GetSpecialDateById")]
        public SpecialDate GetSpecialDateById([FromQuery]int id)
        {
            string query = "SELECT * FROM SpecialDateTable WHERE Id=@Id;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.QueryFirstOrDefault<SpecialDate>(query, new { id });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return null;
                }
            }
        }

        [HttpPost]
        [Route("CreateSpecialDate")]
        public int CreateSpecialDate()
        {
            SpecialDate specialdate = new SpecialDate();
            using (StreamReader sr = new StreamReader(Request.Body))
            {
                specialdate = JsonConvert.DeserializeObject<SpecialDate>(sr.ReadToEnd());
            }
            if (specialdate != null)
            {
                string query = "IF NOT EXISTS (SELECT * FROM SpecialDateTable WHERE Name=@Name AND Date=@Date) " +
                "INSERT INTO SpecialDateTable (Name, Date) VALUES (@Name, @Date);";
                using (SqlConnection conn = new SqlConnection(Connection.ConnString))
                {
                    try
                    {
                        conn.Open();
                        return conn.Execute(query, specialdate);
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
        [Route("UpdateSpecialDate")]
        public int UpdateSpecialDate()
        {
            SpecialDate specialdate = new SpecialDate();
            using (StreamReader sr = new StreamReader(Request.Body))
            {
                specialdate = JsonConvert.DeserializeObject<SpecialDate>(sr.ReadToEnd());
            }
            if (specialdate != null)
            {
                string query = "UPDATE SpecialDateTable SET Name=@Name, Date=@Date WHERE Id=@Id;";
                using (SqlConnection conn = new SqlConnection(Connection.ConnString))
                {
                    try
                    {
                        conn.Open();
                        return conn.Execute(query, specialdate);
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
        [Route("DeleteSpecialDate")]
        public int DeleteSpecialDate([FromQuery]int id)
        {
            if (id > 0)
            {
                string query = "DELETE FROM SpecialDateTable WHERE Id=@Id;";
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

        [HttpGet]
        [Route("GetSkills")]
        public List<Skill> GetSkills()
        {
            string query = "SELECT * FROM SkillTable;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.Query<Skill>(query).ToList();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return new List<Skill>();
                }
            }
        }

        [HttpGet]
        [Route("GetSkillById")]
        public Skill GetSkillById([FromQuery]int id)
        {
            string query = "SELECT * FROM SkillTable WHERE Id=@Id;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.QueryFirstOrDefault<Skill>(query, new { id });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return null;
                }
            }
        }

        [HttpGet]
        [Route("GetSkillsByRole")]
        public List<string> GetSkillsByRole([FromQuery]string role)
        {
            string query = "SELECT Name FROM SkillTable WHERE Role=@Role;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.Query<string>(query, new { role }).ToList();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return new List<string>();
                }
            }
        }

        [HttpPost]
        [Route("CreateSkill")]
        public int CreateSkill()
        {
            Skill skill = new Skill();
            using (StreamReader sr = new StreamReader(Request.Body))
            {
                skill = JsonConvert.DeserializeObject<Skill>(sr.ReadToEnd());
            }
            if (skill != null)
            {
                string query = "IF NOT EXISTS (SELECT * FROM SkillTable WHERE Role=@Role AND Name=@Name) " +
                "INSERT INTO SkillTable (Role, Name) VALUES (@Role, @Name);";
                using (SqlConnection conn = new SqlConnection(Connection.ConnString))
                {
                    try
                    {
                        conn.Open();
                        return conn.Execute(query, skill);
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
        [Route("UpdateSkill")]
        public int UpdateSkill()
        {
            Skill skill = new Skill();
            using (StreamReader sr = new StreamReader(Request.Body))
            {
                skill = JsonConvert.DeserializeObject<Skill>(sr.ReadToEnd());
            }
            if (skill != null)
            {
                string query = "UPDATE SkillTable SET Name=@Name WHERE Id=@Id;";
                using (SqlConnection conn = new SqlConnection(Connection.ConnString))
                {
                    try
                    {
                        conn.Open();
                        return conn.Execute(query, skill);
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
        [Route("DeleteSkill")]
        public int DeleteSkill([FromQuery]int id)
        {
            if (id > 0)
            {
                string query = "DELETE FROM SkillTable WHERE Id=@Id;";
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

        [HttpGet]
        [Route("GetSites")]
        public List<Site> GetSites()
        {
            return GetSitesStatic();
        }

        public static List<Site> GetSitesStatic()
        {
            string query = "SELECT * FROM SiteTable;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.Query<Site>(query).ToList();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return new List<Site>();
                }
            }
        }

        [HttpGet]
        [Route("GetSiteById")]
        public Site GetSiteById([FromQuery]int id)
        {
            string query = "SELECT * FROM SiteTable WHERE Id=@Id;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.QueryFirstOrDefault<Site>(query, new { id });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return null;
                }
            }
        }

        [HttpPost]
        [Route("CreateSite")]
        public int CreateSite()
        {
            Site site = new Site();
            using (StreamReader sr = new StreamReader(Request.Body))
            {
                site = JsonConvert.DeserializeObject<Site>(sr.ReadToEnd());
            }
            if (site != null)
            {
                string query = "IF NOT EXISTS (SELECT * FROM SiteTable WHERE Name=@Name AND Type=@Type) " +
                "INSERT INTO SiteTable (Name, Type, Times) VALUES (@Name, @Type, @Times);";
                using (SqlConnection conn = new SqlConnection(Connection.ConnString))
                {
                    try
                    {
                        conn.Open();
                        return conn.Execute(query, site);
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
        [Route("UpdateSite")]
        public int UpdateSite()
        {
            Site site = new Site();
            using (StreamReader sr = new StreamReader(Request.Body))
            {
                site = JsonConvert.DeserializeObject<Site>(sr.ReadToEnd());
            }
            if (site != null)
            {
                string query = "UPDATE SiteTable SET Name=@Name, Type=@Type, Times=@Times WHERE Id=@Id;";
                using (SqlConnection conn = new SqlConnection(Connection.ConnString))
                {
                    try
                    {
                        conn.Open();
                        return conn.Execute(query, site);
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
        [Route("DeleteSite")]
        public int DeleteSite([FromQuery]int id)
        {
            if (id > 0)
            {
                string query = "DELETE FROM SiteTable WHERE Id=@Id;";
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

        [HttpGet]
        [Route("GetRoles")]
        public List<Role> GetRoles()
        {
            string query = "SELECT * FROM RoleTable;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.Query<Role>(query).ToList();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return new List<Role>();
                }
            }
        }

        [HttpGet]
        [Route("GetRoleNames")]
        public List<string> GetRoleNames()
        {
            string query = "SELECT Name FROM RoleTable;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.Query<string>(query).ToList();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return new List<string>();
                }
            }
        }

        [HttpGet]
        [Route("GetRoleById")]
        public Role GetRoleById([FromQuery]int id)
        {
            string query = "SELECT * FROM RoleTable WHERE Id=@Id;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.QueryFirstOrDefault<Role>(query, new { id });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return null;
                }
            }
        }

        [HttpPost]
        [Route("CreateRole")]
        public int CreateRole()
        {
            Role site = new Role();
            using (StreamReader sr = new StreamReader(Request.Body))
            {
                site = JsonConvert.DeserializeObject<Role>(sr.ReadToEnd());
            }
            if (site != null)
            {
                string query = "IF NOT EXISTS (SELECT * FROM RoleTable WHERE Name=@Name) " +
                "INSERT INTO RoleTable (Name) VALUES (@Name);";
                using (SqlConnection conn = new SqlConnection(Connection.ConnString))
                {
                    try
                    {
                        conn.Open();
                        return conn.Execute(query, site);
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
        [Route("UpdateRole")]
        public int UpdateRole()
        {
            Role site = new Role();
            using (StreamReader sr = new StreamReader(Request.Body))
            {
                site = JsonConvert.DeserializeObject<Role>(sr.ReadToEnd());
            }
            if (site != null)
            {
                string query = "UPDATE RoleTable SET Name=@Name WHERE Id=@Id;";
                using (SqlConnection conn = new SqlConnection(Connection.ConnString))
                {
                    try
                    {
                        conn.Open();
                        return conn.Execute(query, site);
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
        [Route("DeleteRole")]
        public int DeleteRole([FromQuery]int id)
        {
            if (id > 0)
            {
                string query = "DELETE FROM RoleTable WHERE Id=@Id;";
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
    }
}
