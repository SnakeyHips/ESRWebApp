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

        [HttpGet("{week}")]
        [Route("GetRoster")]
        public List<Employee> GetRoster(double week)
        {
            string query = "SELECT Week, EmployeeId as Id, EmployeeName as Name, Role, ContractHours, " +
                "AppointedHours, AbsenceHours, LowRateUHours, HighRateUHours, OvertimeHours " +
                "FROM RosterTable WHERE Week=@Week;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.Query<Employee>(query, new { week }).ToList();
                }
                catch
                {
                    return new List<Employee>();
                }
            }
        }

        [HttpGet("{week}/{id}")]
        [Route("GetEmployeeRoster")]
        public Employee GetEmployeeRoster(double week, int id)
        {
            string query = "SELECT * FROM RosterTable WHERE Week=@Week AND EmployeeId=@Id;";
            using (SqlConnection conn = new SqlConnection(Connection.ConnString))
            {
                try
                {
                    conn.Open();
                    return conn.QueryFirst<Employee>(query, new { week });
                }
                catch
                {
                    return null;
                }
            }
        }

        public int CreateRoster(int id, double appointed, double absence, double lowrateu, double highrateu, double overtime, double week)
        {
            string query = "INSERT INTO RosterTable " +
                "(Week, EmployeeId, EmployeeName, Role, ContractHours, AppointedHours, AbsenceHours, LowRateUHours, HighRateUHours, OvertimeHours)" +
                " VALUES (@Week, @Id, @Name, @Role, @ContractHours, @Appointed, @Absence, @Lowrateu, @Highrateu, @Overtime);";
            Employee employee = EmployeeController.GetById(id);
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

        public int UpdateAbsence(int id, double absence, double week)
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
            Session Before;
            Session After;
            using (StreamReader sr = new StreamReader(Request.Body))
            {
                List<Session> sessions = JsonConvert.DeserializeObject<List<Session>>(sr.ReadToEnd());
                Before = sessions[0];
                After = sessions[1];
            }
            switch (After.Holiday)
            {
                case 0:
                    UpdateRoster(Before, After);
                    System.Diagnostics.Debug.WriteLine("normal");
                    break;
                case 1:
                    UpdateRosterLowRate(Before, After);
                    System.Diagnostics.Debug.WriteLine("lowrate");
                    break;
                case 2:
                    UpdateRosterHighRate(Before, After);
                    System.Diagnostics.Debug.WriteLine("highrate");
                    break;
            }
            return SessionController.UpdateStaff(After);
        }

        public void UpdateRoster(Session Before, Session After)
        {

            After.StaffCount = 0;
            double Week = EmployeeController.GetWeek(After.Date);
            //First check if there is a selected item
            if (After.SV1Id != 0)
            {
                if (Before.SV1Id == 0)
                {
                    //Add onto new staff's appointed hours/New record created in sql method
                    After.StaffCount++;
                    UpdateHours(After.SV1Id, After.SV1LOD, After.SV1UNS, After.SV1OT, Week);
                }
                //Check if same staff selected
                else if (Before.SV1Id == After.SV1Id)
                {
                    //Check if different times from before
                    if (Before.SV1LOD != After.SV1LOD)
                    {
                        //If so, update times and appointed hours to use times selected
                        double appointed = After.SV1LOD - Before.SV1LOD;
                        UpdateAppointed(After.SV1Id, appointed, Week);
                    }
                    //Check if different unsocial from before
                    if (Before.SV1UNS != After.SV1UNS)
                    {
                        //If so, update times and appointed hours to use times selected
                        double unsocial = After.SV1UNS - Before.SV1UNS;
                        UpdateUnsocial(After.SV1Id, unsocial, Week);
                    }
                    //Check if different ot from before
                    if (Before.SV1OT != After.SV1OT)
                    {
                        //If so, update times and appointed hours to use times selected
                        double overtime = After.SV1OT - Before.SV1OT;
                        UpdateOvertime(After.SV1Id, overtime, Week);
                    }
                }
                //Check if different staff selected
                else if (Before.SV1Id != After.SV1Id)
                {
                    //Update old staff's to remove hours
                    UpdateHours(Before.SV1Id, -Before.SV1LOD, -Before.SV1UNS, -Before.SV1OT, Week);
                    //Add onto new staff's appointed hours/New record created in sql method
                    UpdateHours(After.SV1Id, After.SV1LOD, After.SV1UNS, After.SV1OT, Week);
                }
            }
            //Else remove if reset has been pressed
            else if (After.SV1Id == 0 && Before.SV1Id != 0)
            {
                //Remove staff's appointed hours
                UpdateHours(Before.SV1Id, -Before.SV1LOD, -Before.SV1UNS, -Before.SV1OT, Week);
            }
        }

        public void UpdateRosterLowRate(Session Before, Session After)
        {
            After.StaffCount = 0;
            double Week = EmployeeController.GetWeek(After.Date);
            //First check if there is a selected item
            if (After.SV1Id != 0)
            {
                if (Before.SV1Id == 0)
                {
                    //Add onto new staff's appointed hours/New record created in sql method
                    After.StaffCount++;
                    UpdateHoursLowRate(After.SV1Id, After.SV1LOD, After.SV1OT, Week);
                }
                //Check if same staff selected
                else if (Before.SV1Id == After.SV1Id)
                {
                    //Check if different times from before
                    if (Before.SV1LOD != After.SV1LOD)
                    {
                        //If so, update times and appointed hours to use times selected
                        double appointed = After.SV1LOD - Before.SV1LOD;
                        UpdateLowRateAppointed(After.SV1Id, appointed, Week);
                    }
                    //Check if different ot from before
                    if (Before.SV1OT != After.SV1OT)
                    {
                        //If so, update times and appointed hours to use times selected
                        double overtime = After.SV1OT - Before.SV1OT;
                        UpdateOvertime(After.SV1Id, overtime, Week);
                    }
                }
                //Check if different staff selected
                else if (Before.SV1Id != After.SV1Id)
                {
                    //Update old staff's to remove hours
                    UpdateHoursLowRate(Before.SV1Id, -Before.SV1LOD, -Before.SV1OT, Week);
                    //Add onto new staff's appointed hours/New record created in sql method
                    UpdateHoursLowRate(After.SV1Id, After.SV1LOD, After.SV1OT, Week);
                }
            }
            //Else remove if reset has been pressed
            else if (After.SV1Id == 0 && Before.SV1Id != 0)
            {
                //Remove staff's appointed hours
                UpdateHoursLowRate(Before.SV1Id, -Before.SV1LOD, -Before.SV1OT, Week);
            }
        }

        public void UpdateRosterHighRate(Session Before, Session After)
        {
            After.StaffCount = 0;
            double Week = EmployeeController.GetWeek(After.Date);
            //First check if there is a selected item
            if (After.SV1Id != 0)
            {
                if (Before.SV1Id == 0)
                {
                    //Add onto new staff's appointed hours/New record created in sql method
                    After.StaffCount++;
                    UpdateHoursHighRate(After.SV1Id, After.SV1LOD, After.SV1OT, Week);
                }
                //Check if same staff selected
                else if (Before.SV1Id == After.SV1Id)
                {
                    //Check if different times from before
                    if (Before.SV1LOD != After.SV1LOD)
                    {
                        //If so, update times and appointed hours to use times selected
                        double appointed = After.SV1LOD - Before.SV1LOD;
                        UpdateHighRateAppointed(After.SV1Id, appointed, Week);
                    }
                    //Check if different ot from before
                    if (Before.SV1OT != After.SV1OT)
                    {
                        //If so, update times and appointed hours to use times selected
                        double overtime = After.SV1OT - Before.SV1OT;
                        UpdateOvertime(After.SV1Id, overtime, Week);
                    }
                }
                //Check if different staff selected
                else if (Before.SV1Id != After.SV1Id)
                {
                    //Update old staff's to remove hours
                    UpdateHoursHighRate(Before.SV1Id, -Before.SV1LOD, -Before.SV1OT, Week);
                    //Add onto new staff's appointed hours/New record created in sql method
                    UpdateHoursHighRate(After.SV1Id, After.SV1LOD, After.SV1OT, Week);
                }
            }
            //Else remove if reset has been pressed
            else if (After.SV1Id == 0 && Before.SV1Id != 0)
            {
                //Remove staff's appointed hours
                UpdateHoursHighRate(Before.SV1Id, -Before.SV1LOD, -Before.SV1OT, Week);
            }
        }
    }
}
