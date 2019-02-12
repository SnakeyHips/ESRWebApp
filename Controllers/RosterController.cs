using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using ERSWebApp.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Newtonsoft.Json;

namespace ERSWebApp.Controllers
{
    [Route("api/[controller]")]
    public class RosterController : Controller
    {
        [HttpGet]
        [Route("GetRosterWeeks")]
        public List<double> GetRosterWeeks()
        {
            string query = "SELECT Week FROM RosterTable;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.Query<double>(query).Distinct().ToList();
                }
                catch
                {
                    return new List<double>();
                }
            }
        }

        [HttpPost]
        [Route("GetRoster")]
        public List<Employee> GetRoster()
        {
            List<double> selectedWeeks = new List<double>();
            List<Employee> employees = new List<Employee>();
            using (StreamReader sr = new StreamReader(Request.Body))
            {
                selectedWeeks = JsonConvert.DeserializeObject<List<double>>(sr.ReadToEnd());
            }
            foreach (double week in selectedWeeks)
            {
                List<Employee> temp = new List<Employee>();
                string query = "SELECT Week, EmployeeId as Id, EmployeeName as Name, Role, ContractHours, " +
                "AppointedHours, AbsenceHours, LowRateUHours, HighRateUHours, OvertimeHours " +
                "FROM RosterTable WHERE Week=@Week;";
                using (SqlConnection conn = new SqlConnection(Connection.ConnString))
                {
                    try
                    {
                        conn.Open();
                        temp = conn.Query<Employee>(query, new { week }).ToList();
                    }
                    catch
                    {
                        //do nothing
                    }
                }
                foreach (Employee t in temp)
                {
                    double difference = t.ContractHours - (t.AppointedHours + t.AbsenceHours);
                    if (difference > 0)
                    {
                        t.NegHours = difference;
                        t.COHours = 0;

                    }
                    else if (difference < 0)
                    {
                        t.NegHours = 0;
                        t.COHours = Math.Abs(difference);
                    }
                    else
                    {
                        t.NegHours = 0;
                        t.COHours = 0;
                    }
                    bool match = false;
                    foreach (Employee e in employees)
                    {
                        if (t.Id == e.Id)
                        {
                            match = true;
                            e.ContractHours += t.ContractHours;
                            e.AppointedHours += t.AppointedHours;
                            e.AbsenceHours += t.AbsenceHours;
                            e.LowRateUHours += t.LowRateUHours;
                            e.HighRateUHours += t.HighRateUHours;
                            e.OvertimeHours += t.OvertimeHours;
                            e.NegHours += t.NegHours;
                            e.COHours += t.COHours;
                            break;
                        }
                    }
                    if (match == false)
                    {
                        employees.Add(t);
                    }
                }
            }
            return employees;
        }

        [HttpGet]
        [Route("GetEmployeeRoster")]
        public Employee GetEmployeeRoster([FromQuery]double week, [FromQuery]int id)
        {
            string query = "SELECT * FROM RosterTable WHERE Week=@Week AND EmployeeId=@Id;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.QueryFirstOrDefault<Employee>(query, new { week });
                }
                catch
                {
                    return null;
                }
            }
        }

        public static int CreateRoster(int id, double appointed, double absence, double lowrateu, double highrateu, double overtime, double week)
        {
            string query = "INSERT INTO RosterTable " +
                "(Week, EmployeeId, EmployeeName, Role, ContractHours, AppointedHours, AbsenceHours, LowRateUHours, HighRateUHours, OvertimeHours)" +
                " VALUES (@Week, @Id, @Name, @Role, @ContractHours, @Appointed, @Absence, @Lowrateu, @Highrateu, @Overtime);";
            Employee employee = EmployeeController.GetByIdStatic(id);
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.Execute(query, new { week, id, employee.Name, employee.Role, employee.ContractHours, appointed, absence, lowrateu, highrateu, overtime });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return -1;
                }
            }
        }

        public int UpdateAppointed(int id, double appointed, double week)
        {
            if (id != 0)
            {
                string query = "UPDATE RosterTable" +
                    " SET AppointedHours=AppointedHours+@Appointed " +
                    " WHERE Week=@Week AND EmployeeId=@Id;";
                int rows = 0;
                using (SqlConnection conn = new SqlConnection(Connection.ConnString))
                {
                    try
                    {
                        conn.Open();
                        rows = conn.Execute(query, new
                        {
                            appointed,
                            week,
                            id
                        });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                    }
                    if (rows == 0)
                    {
                        //Add in new roster if update fails
                        CreateRoster(id, appointed, 0.0, 0.0, 0.0, 0.0, week);
                    }
                }
                return rows;
            }
            return -1;
        }

        public int UpdateLowRateAppointed(int id, double appointed, double week)
        {
            if (id != 0)
            {
                string query = "UPDATE RosterTable" +
                    " SET AppointedHours=AppointedHours+@Appointed, LowRateUHours=LowRateUHours+@Appointed" +
                    " WHERE Week=@Week AND EmployeeId=@Id;";
                using (SqlConnection conn = new SqlConnection(Connection.ConnString))
                {
                    try
                    {
                        conn.Open();
                        return conn.Execute(query, new { appointed, week, id });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                        return -1;
                    }
                }
            }
            return -1;
        }

        public int UpdateHighRateAppointed(int id, double appointed, double week)
        {
            if (id != 0)
            {
                string query = "UPDATE RosterTable" +
                    " SET AppointedHours=AppointedHours+@Appointed, HighRateUHours=HighRateUHours+@Appointed" +
                    " WHERE Week=@Week AND EmployeeId=@Id;";
                using (SqlConnection conn = new SqlConnection(Connection.ConnString))
                {
                    try
                    {
                        conn.Open();
                        return conn.Execute(query, new { appointed, week, id });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                        return -1;
                    }
                }
            }
            return -1;
        }

        public static int UpdateAbsence(int id, double absence, double week)
        {
            if (id != 0)
            {
                string query = "UPDATE RosterTable" +
                    " SET AbsenceHours=AbsenceHours+@Absence " +
                    " WHERE Week=@Week AND EmployeeId=@Id;";
                int rows = 0;
                using (SqlConnection conn = new SqlConnection(Connection.ConnString))
                {
                    try
                    {
                        conn.Open();
                        rows = conn.Execute(query, new { absence, week, id });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                    }
                    if (rows == 0)
                    {
                        //Add in new roster if update fails
                        CreateRoster(id, 0.0, absence, 0.0, 0.0, 0.0, week);
                    }
                    return rows;
                }
            }
            return -1;
        }

        public int UpdateUnsocial(int id, double unsocial, double week)
        {
            if (id != 0)
            {
                string query = "UPDATE RosterTable" +
                    " SET LowRateUHours=LowRateUHours+@Unsocial " +
                    " WHERE Week=@Week AND EmployeeId=@Id;";
                using (SqlConnection conn = new SqlConnection(Connection.ConnString))
                {
                    try
                    {
                        conn.Open();
                        return conn.Execute(query, new { unsocial, week, id });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                        return -1;
                    }
                }
            }
            return -1;
        }

        public int UpdateOvertime(int id, double overtime, double week)
        {
            if (id != 0)
            {
                string query = "UPDATE RosterTable" +
                    " SET OvertimeHours=OvertimeHours+@Overtime " +
                    " WHERE Week=@Week AND EmployeeId=@Id;";
                using (SqlConnection conn = new SqlConnection(Connection.ConnString))
                {
                    try
                    {
                        conn.Open();
                        return conn.Execute(query, new { overtime, week, id });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                        return -1;
                    }
                }
            }
            return -1;
        }

        public int UpdateHours(int id, double appointed, double unsocial, double overtime, double week)
        {
            if (id != 0)
            {
                string query = "UPDATE RosterTable" +
                    " SET AppointedHours=AppointedHours+@Appointed, LowRateUHours=LowRateUHours+@Unsocial, OvertimeHours=OvertimeHours+@Overtime " +
                    " WHERE Week=@Week AND EmployeeId=@Id;";
                int rows = 0;
                using (SqlConnection conn = new SqlConnection(Connection.ConnString))
                {
                    try
                    {
                        conn.Open();
                        rows = conn.Execute(query, new { appointed, unsocial, overtime, week, id });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                    }
                    if (rows == 0)
                    {
                        //Add in new roster if update fails
                        CreateRoster(id, appointed, 0.0, unsocial, 0.0, overtime, week);
                    }
                }
            }
            return -1;
        }

        public int UpdateHoursLowRate(int id, double appointed, double overtime, double week)
        {
            if (id != 0)
            {
                string query = "UPDATE RosterTable" +
                    " SET AppointedHours=AppointedHours+@Appointed, LowRateUHours=LowRateUHours+@Appointed, OvertimeHours=OvertimeHours+@Overtime " +
                    " WHERE Week=@Week AND EmployeeId=@Id;";
                int rows = 0;
                using (SqlConnection conn = new SqlConnection(Connection.ConnString))
                {
                    try
                    {
                        conn.Open();
                        rows = conn.Execute(query, new { appointed, overtime, week, id });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                    }
                    if (rows == 0)
                    {
                        //Add in new roster if update fails
                        CreateRoster(id, appointed, 0.0, appointed, 0.0, overtime, week);
                    }
                }
            }
            return -1;
        }

        public int UpdateHoursHighRate(int id, double appointed, double overtime, double week)
        {
            if (id != 0)
            {
                string query = "UPDATE RosterTable" +
                    " SET AppointedHours=AppointedHours+@Appointed, HighRateUHours=HighRateUHours+@Appointed, " +
                    "OvertimeHours=OvertimeHours+@Overtime " +
                    " WHERE Week=@Week AND EmployeeId=@Id;";
                int rows = 0;
                using (SqlConnection conn = new SqlConnection(Connection.ConnString))
                {
                    try
                    {
                        conn.Open();
                        rows = conn.Execute(query, new { appointed, overtime, week, id });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                    }
                    if (rows == 0)
                    {
                        //Add in new roster if update fails
                        CreateRoster(id, appointed, 0.0, 0.0, appointed, overtime, week);
                    }
                }
            }
            return -1;
        }

        public int DeleteRoster(int id, double week)
        {
            if (id > 0)
            {
                string query = "DELETE FROM RosterTable WHERE Week=@Week AND EmployeeId=@Id;";
                using (SqlConnection conn = new SqlConnection(Connection.ConnString))
                {
                    try
                    {
                        conn.Open();
                        return conn.Execute(query, new { id, week });
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
            try
            {
                Session before;
                Session after;
                using (StreamReader sr = new StreamReader(Request.Body))
                {
                    List<Session> sessions = JsonConvert.DeserializeObject<List<Session>>(sr.ReadToEnd());
                    before = sessions[0];
                    after = sessions[1];
                }
                // Remove any blank employees
                for(int i = after.Employees.Count-1; i >= 0; i--)
                {
                    if (after.Employees[i].EmployeeId < 1)
                    {
                        after.Employees.RemoveAt(i);
                    }
                }
                switch (after.Holiday)
                {
                    case 0:
                        UpdateRoster(before, after);
                        break;
                    case 1:
                        UpdateRosterLowRate(before, after);
                        break;
                    case 2:
                        UpdateRosterHighRate(before, after);
                        break;
                }
                // Check if state complete by comparing StaffCount with Template
                List<string> templateRoles = AdminController.GetTemplateByNameStatic(after.Template).Split(",").ToList();
                // First remove and roles which shouldn't be included such as RN
                for (int j = templateRoles.Count - 1; j >= 0; j--)
                {
                    if (templateRoles[j].Equals("RN"))
                    {
                        templateRoles.RemoveAt(j);
                    }
                }
                // Then check if StaffCount is more than/equal to left over roles
                if(after.StaffCount >= templateRoles.Count)
                {
                    after.State = 1;
                }
                else
                {
                    after.State = 0;
                }
                // Update SessionEmployee data
                int rows = 0;
                if (after != null)
                {
                    string query = "UPDATE SessionTable " +
                        "SET LOD=@LOD, OCC=@OCC, Estimate=@Estimate, StaffCount=@StaffCount, State=@State, Note=@Note WHERE Id=@Id;";
                    string queryName = "SELECT Name FROM EmployeeTable WHERE Id=@Id;";
                    string queryDelete = "DELETE FROM SessionEmployeeTable WHERE SessionId=@AfterId;";
                    string queryList = "INSERT INTO SessionEmployeeTable (SessionId, SessionDate, SessionSite, EmployeeId," +
                        " EmployeeName, EmployeeRole, EmployeeLOD, EmployeeUNS, EmployeeOT) " +
                    "VALUES (@SessionId, @SessionDate, @SessionSite, @EmployeeId, @EmployeeName, @EmployeeRole, @EmployeeLOD, @EmployeeUNS, @EmployeeOT);";
                    using (SqlConnection conn = new SqlConnection(Connection.ConnString))
                    {
                        try
                        {
                            conn.Open();
                            rows += conn.Execute(query, after);
                            foreach (SessionEmployee e in after.Employees)
                            {
                                e.EmployeeName = conn.QueryFirstOrDefault<string>(queryName, new { Id = e.EmployeeId });
                            }
                            conn.Execute(queryDelete, new { AfterId = after.Id });
                            rows += conn.Execute(queryList, after.Employees);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex);
                            rows = -1;
                        }
                    }
                }
                return rows;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return -1;
            }
        }

        public void UpdateRoster(Session Before, Session After)
        {
            double Week = GetWeek(After.Date);
            After.StaffCount = 0;

            for(int i = 0; i < After.Employees.Count; i++)
            {
                bool match = false;
                //First check for any matches and see if any hours are different
                for(int j = Before.Employees.Count - 1; j >= 0; j--)
                {
                    if(Before.Employees[j].EmployeeId == After.Employees[i].EmployeeId)
                    {
                        match = true;
                        if (Before.Employees[j].EmployeeLOD != After.Employees[i].EmployeeLOD)
                        {
                            double appointed = After.Employees[i].EmployeeLOD - Before.Employees[j].EmployeeLOD;
                            UpdateAppointed(After.Employees[i].EmployeeId, appointed, Week);
                        }
                        if (Before.Employees[j].EmployeeUNS != After.Employees[i].EmployeeUNS)
                        {
                            double unsocial = After.Employees[i].EmployeeUNS - Before.Employees[j].EmployeeUNS;
                            UpdateUnsocial(After.Employees[i].EmployeeId, unsocial, Week);
                        }
                        if (Before.Employees[j].EmployeeOT != After.Employees[i].EmployeeOT)
                        {
                            double overtime = After.Employees[i].EmployeeOT - Before.Employees[j].EmployeeOT;
                            UpdateOvertime(After.Employees[i].EmployeeId, overtime, Week);
                        }
                        //Remove so Before.employees then only contains no matches
                        Before.Employees.RemoveAt(j);
                    }
                }
                //If no match, add to RosterTable
                if (!match)
                {
                    UpdateHours(After.Employees[i].EmployeeId, After.Employees[i].EmployeeLOD, After.Employees[i].EmployeeUNS, After.Employees[i].EmployeeOT, Week);
                }
                //Add to staff count if not RN
                if (After.Employees[i].EmployeeRole != "RN")
                {
                    After.StaffCount++;
                }
            }
            //Remove hours for any left in Before.employees as these weren't matched
            for(int i = 0; i < Before.Employees.Count; i++)
            {
                UpdateHours(Before.Employees[i].EmployeeId, -Before.Employees[i].EmployeeLOD, -Before.Employees[i].EmployeeUNS, -Before.Employees[i].EmployeeOT, Week);
            }
        }

        public void UpdateRosterLowRate(Session Before, Session After)
        {
            double Week = GetWeek(After.Date);
            After.StaffCount = 0;

            for (int i = 0; i < After.Employees.Count; i++)
            {
                bool match = false;
                //First check for any matches and see if any hours are different
                for (int j = Before.Employees.Count - 1; j >= 0; j--)
                {
                    if (Before.Employees[j].EmployeeId == After.Employees[i].EmployeeId)
                    {
                        match = true;
                        if (Before.Employees[j].EmployeeLOD != After.Employees[i].EmployeeLOD)
                        {
                            double appointed = After.Employees[i].EmployeeLOD - Before.Employees[j].EmployeeLOD;
                            UpdateLowRateAppointed(After.Employees[i].EmployeeId, appointed, Week);
                        }
                        if (Before.Employees[j].EmployeeOT != After.Employees[i].EmployeeOT)
                        {
                            double overtime = After.Employees[i].EmployeeOT - Before.Employees[j].EmployeeOT;
                            UpdateOvertime(After.Employees[i].EmployeeId, overtime, Week);
                        }
                        //Remove so Before.employees then only contains no matches
                        Before.Employees.RemoveAt(j);
                    }
                }
                //If no match, add to RosterTable
                if (!match)
                {
                    UpdateHoursLowRate(After.Employees[i].EmployeeId, After.Employees[i].EmployeeLOD, After.Employees[i].EmployeeOT, Week);
                }
                //Add to staff count if not RN
                if (After.Employees[i].EmployeeRole != "RN")
                {
                    After.StaffCount++;
                }
            }
            //Remove hours for any left in Before.employees as these weren't matched
            for (int i = 0; i < Before.Employees.Count; i++)
            {
                UpdateHoursLowRate(Before.Employees[i].EmployeeId, -Before.Employees[i].EmployeeLOD, -Before.Employees[i].EmployeeOT, Week);
            }
        }

        public void UpdateRosterHighRate(Session Before, Session After)
        {
            double Week = GetWeek(After.Date);
            After.StaffCount = 0;

            for (int i = 0; i < After.Employees.Count; i++)
            {
                bool match = false;
                //First check for any matches and see if any hours are different
                for (int j = Before.Employees.Count - 1; j >= 0; j--)
                {
                    if (Before.Employees[j].EmployeeId == After.Employees[i].EmployeeId)
                    {
                        match = true;
                        if (Before.Employees[j].EmployeeLOD != After.Employees[i].EmployeeLOD)
                        {
                            double appointed = After.Employees[i].EmployeeLOD - Before.Employees[j].EmployeeLOD;
                            UpdateHighRateAppointed(After.Employees[i].EmployeeId, appointed, Week);
                        }
                        if (Before.Employees[j].EmployeeOT != After.Employees[i].EmployeeOT)
                        {
                            double overtime = After.Employees[i].EmployeeOT - Before.Employees[j].EmployeeOT;
                            UpdateOvertime(After.Employees[i].EmployeeId, overtime, Week);
                        }
                        //Remove so Before.employees then only contains no matches
                        Before.Employees.RemoveAt(j);
                    }
                }
                //If no match, add to RosterTable
                if (!match)
                {
                    UpdateHoursHighRate(After.Employees[i].EmployeeId, After.Employees[i].EmployeeLOD, After.Employees[i].EmployeeOT, Week);
                }
                //Add to staff count if not RN
                if (After.Employees[i].EmployeeRole != "RN")
                {
                    After.StaffCount++;
                }
            }
            //Remove hours for any left in Before.employees as these weren't matched
            for (int i = 0; i < Before.Employees.Count; i++)
            {
                UpdateHoursHighRate(Before.Employees[i].EmployeeId, -Before.Employees[i].EmployeeLOD, -Before.Employees[i].EmployeeOT, Week);
            }
        }

        public static double GetWeek(string stringdate)
        {
            DateTime date = DateTime.Parse(stringdate);
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;
            return double.Parse(cal.GetWeekOfYear(date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek).ToString()
                + "." + date.Year.ToString());
        }
    }
}
