using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ERSWebApp.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Newtonsoft.Json;
using System.Globalization;

namespace ERSWebApp.Controllers
{
    [Route("api/[controller]")]
    public class EmployeeController : Controller
    {
        [HttpGet]
        [Route("GetEmployees")]
        public List<Employee> GetEmployees()
        {
            string query = "SELECT * FROM EmployeeTable;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return new List<Employee>(conn.Query<Employee>(query).ToList());
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return new List<Employee>();
                }
            }
        }

        [HttpPost]
        [Route("Create")]
        public int Create()
        {
            Employee employee = new Employee();
            using (StreamReader sr = new StreamReader(Request.Body))
            {
                employee = JsonConvert.DeserializeObject<Employee>(sr.ReadToEnd());
            }
            if (employee != null)
            {
                string query = "INSERT INTO EmployeeTable (Id, Name, Role, Skill, Address, Number, ContractHours, WorkPattern)" +
                    "VALUES (@Id, @Name, @Role, @Skill, @Address, @Number, @ContractHours, @WorkPattern);";
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

        [HttpGet("{id}")]
        [Route("GetById")]
        public static Employee GetById(int id)
        {
            string query = "SELECT * FROM EmployeeTable WHERE Id=@Id;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.QuerySingle<Employee>(query, new { id });
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
            Employee employee = new Employee();
            using (StreamReader sr = new StreamReader(Request.Body))
            {
                employee = JsonConvert.DeserializeObject<Employee>(sr.ReadToEnd());
            }
            if (employee != null)
            {
                string query = "UPDATE EmployeeTable SET Role=@Role, Skill=@Skill, Address=@Address, Number=@Number, " +
                    "ContractHours=@ContractHours, WorkPattern=@WorkPattern WHERE Id=@Id;";
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

        [HttpDelete("{id}")]
        [Route("Delete")]
        public int Delete(int id)
        {
            if (id > 0)
            {
                string query = "DELETE FROM EmployeeTable WHERE Id=@Id;";
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

        [HttpGet("{dayofweek}")]
        [Route("GetAvailable")]
        public List<Employee> GetAvailable(string dayofweek)
        {
            List<Employee> available = GetEmployees();
            foreach (Employee e in available)
            { 
                if (e.Status == "Okay" && e.WorkPattern.Contains(dayofweek.Substring(0, 3)))
                {
                    available.Add(e);
                }
            }
            return available;
        }

        [HttpGet("{stringdate}")]
        [Route("GetWeek")]
        public static double GetWeek(string stringdate)
        {
            DateTime date = DateTime.Parse(stringdate);
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;
            return double.Parse(cal.GetWeekOfYear(date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek).ToString()
                + "." + date.Year.ToString());
        }

        //public void EmployeeCSV()
        //{
        //    List<Employee> employees = new List<Employee>();
        //    StreamReader file = new StreamReader("C:\\Source\\ERSWebApp\\ESRWebApp\\staffs.csv");
        //    string line;
        //    try
        //    {
        //        while ((line = file.ReadLine()) != null)
        //        {
        //            string[] array = line.Split('/');
        //            employees.Add(new Employee()
        //            {
        //                Id = Convert.ToInt32(array[0]),
        //                Name = array[1],
        //                Role = array[2],
        //                Skill = array[3],
        //                Address = array[4],
        //                Number = array[5],
        //                ContractHours = Convert.ToDouble(array[6]),
        //                WorkPattern = array[7]
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine(ex);
        //    }
        //    if(employees.Count > 0)
        //    {
        //        string query = "INSERT INTO EmployeeTable (Id, Name, Role, Skill, Address, Number, ContractHours, WorkPattern)" +
        //            "VALUES (@Id, @Name, @Role, @Skill, @Address, @Number, @ContractHours, @WorkPattern);";
        //        using (SqlConnection conn = new SqlConnection(connString))
        //        {
        //            try
        //            {
        //                conn.Open();
        //                conn.Execute(query, employees);
        //            }
        //            catch (Exception ex)
        //            {
        //                System.Diagnostics.Debug.WriteLine(ex);
        //            }
        //        }
        //    }
        //}
    }
}
