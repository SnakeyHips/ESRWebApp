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

        [HttpGet("{id}")]
        [Route("GetSpecialDateById")]
        public SpecialDate GetSpecialDateById(int id)
        {
            string query = "SELECT * FROM SpecialDateTable WHERE Id=@Id;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.QuerySingle<SpecialDate>(query, new { id });
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

        [HttpDelete("{id}")]
        [Route("DeleteSpecialDate")]
        public int DeleteSpecialDate(int id)
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

        [HttpGet("{id}")]
        [Route("GetSiteById")]
        public Site GetSiteById(int id)
        {
            string query = "SELECT * FROM SiteTable WHERE Id=@Id;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.QuerySingle<Site>(query, new { id });
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

        [HttpDelete("{id}")]
        [Route("DeleteSite")]
        public int DeleteSite(int id)
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
    }
}
