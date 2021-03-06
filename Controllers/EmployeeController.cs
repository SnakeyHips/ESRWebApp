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
    public class EmployeeController : Controller
    {
        [HttpGet]
        [Route("GetEmployees")]
        public List<Employee> GetEmployees([FromQuery]string date)
        {
            string query = "SELECT * FROM EmployeeTable;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    List<Employee> employees = conn.Query<Employee>(query).ToList();
                    GetStatuses(employees, AbsenceController.GetAbsencesStatic(date));
                    return employees;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return new List<Employee>();
                }
            }
        }

        [HttpGet]
        [Route("GetTeamEmployees")]
        public List<Employee> GetTeamEmployees()
        {
            string query = "SELECT * FROM EmployeeTable;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    List<Employee> employees = conn.Query<Employee>(query).ToList();
                    return employees;
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

        [HttpGet]
        [Route("GetById")]
        public Employee GetById([FromQuery]int id)
        {
            Employee temp = GetByIdStatic(id);
            if(temp == null)
            {
                temp = new Employee();
            }
            return temp;
        }

        public static Employee GetByIdStatic(int id)
        {
            string query = "SELECT * FROM EmployeeTable WHERE Id=@Id;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.QueryFirstOrDefault<Employee>(query, new { id });
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

        [HttpDelete]
        [Route("Delete")]
        public int Delete([FromQuery]int id)
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

        [HttpGet]
        [Route("GetAvailable")]
        public List<Employee> GetAvailable([FromQuery]string date, [FromQuery]string day, [FromQuery] int sessionid)
        {
            List<Employee> available = new List<Employee>();
            foreach (Employee e in GetEmployees(date))
            {
                if (e.WorkPattern.Contains(day.Substring(0, 3)))
                {
                    if (CheckEmployeeSession(e.Id, sessionid, date))
                    {
                        if (e.Status == "Okay")
                        {
                            available.Add(e);
                        }
                        else
                        {
                            if (e.Status.Contains("Part"))
                            {
                                available.Add(e);
                            }
                        }
                    }
                }
            }
            return available;
        }

        [HttpGet()]
        [Route("GetEmployeeSessions")]
        public List<Session> GetEmployeeSessions([FromQuery]int employeeid, [FromQuery]string startdate, [FromQuery]string enddate)
        {
            List<Session> sessions = new List<Session>();
            string query = "SELECT SessionId FROM SessionEmployeeTable WHERE SessionDate BETWEEN @StartDate AND @EndDate AND EmployeeId=@EmployeeId;";
            string querySession = "SELECT * FROM SessionTable WHERE Id=@SessionId";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    List<int> sessionids = conn.Query<int>(query, new { startdate, enddate, employeeid }).ToList();
                    foreach(int sessionid in sessionids)
                    {
                        sessions.Add(conn.QueryFirstOrDefault<Session>(querySession, new { sessionid }));
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }
            return sessions;
        }

        public static bool CheckEmployeeSession(int employeeid, int sessionid, string date)
        {
            string query = "SELECT SessionId FROM SessionEmployeeTable WHERE SessionDate=@Date AND EmployeeId=@EmployeeId;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    int id = conn.QueryFirstOrDefault<int>(query, new { employeeid, date });
                    if (id < 1 || id == sessionid)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return false;
                }
            }
        }

        public static void GetStatuses(List<Employee> employees, List<Absence> absences)
        {
            for (int i = 0; i < employees.Count; i++)
            {
                string status = "Okay";
                for (int j = 0; j < absences.Count; j++)
                {
                    if (absences[j].EmployeeId == employees[i].Id)
                    {
                        status = absences[j].Type;
                        if(absences[j].PartDay == "Yes")
                        {
                            status += " - Part";
                        }
                        break;
                    }
                }
                employees[i].Status = status;
            }
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
