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
                        t.COHours = difference;
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
                            e.NegHours += e.NegHours;
                            e.COHours += e.COHours;
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
            Session before;
            Session after;
            using (StreamReader sr = new StreamReader(Request.Body))
            {
                List<Session> sessions = JsonConvert.DeserializeObject<List<Session>>(sr.ReadToEnd());
                before = sessions[0];
                after = sessions[1];
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
            //check if state complete
            if (after.Chairs < 9)
            {
                if (after.Chairs < 6 && after.Type.Equals("MDC"))
                {
                    if(after.StaffCount < 3)
                    {
                        after.State = 0;
                    }
                    else
                    {
                        after.State = 1;
                    }
                }
                else
                {
                    if(after.StaffCount < 5)
                    {
                        after.State = 0;
                    }
                    else
                    {
                        after.State = 1;
                    }
                }
            }
            else
            {
                if(after.StaffCount < 6)
                {
                    after.State = 0;
                }
                else
                {
                    after.State = 1;
                }
            }
            return SessionController.UpdateStaff(after);
        }

        public void UpdateRoster(Session Before, Session After)
        {
            System.Diagnostics.Debug.WriteLine(After.StaffCount.ToString());
            double Week = GetWeek(After.Date);
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
                After.StaffCount--;
            }

            //Same for drivers
            if (After.DRI1Id != 0)
            {
                if (Before.DRI1Id == 0)
                {
                    After.StaffCount++;
                    UpdateHours(After.DRI1Id, After.DRI1LOD, After.DRI1UNS, After.DRI1OT, Week);
                }
                else if (Before.DRI1Id == After.DRI1Id)
                {
                    if (Before.DRI1LOD != After.DRI1LOD)
                    {
                        double appointed = After.DRI1LOD - Before.DRI1LOD;
                        UpdateAppointed(After.DRI1Id, appointed, Week);
                    }
                    if (Before.DRI1UNS != After.DRI1UNS)
                    {
                        double unsocial = After.DRI1UNS - Before.DRI1UNS;
                        UpdateUnsocial(After.DRI1Id, unsocial, Week);
                    }
                    if (Before.DRI1OT != After.DRI1OT)
                    {
                        double overtime = After.DRI1OT - Before.DRI1OT;
                        UpdateOvertime(After.DRI1Id, overtime, Week);
                    }
                }
                else if (Before.DRI1Id != After.DRI1Id)
                {
                    UpdateHours(Before.DRI1Id, -Before.DRI1LOD, -Before.DRI1UNS, -Before.DRI1OT, Week);
                    UpdateHours(After.DRI1Id, After.DRI1LOD, After.DRI1UNS, After.DRI1OT, Week);
                }
            }
            else if (After.DRI1Id == 0 && Before.DRI1Id != 0)
            {
                UpdateHours(Before.DRI1Id, -Before.DRI1LOD, -Before.DRI1UNS, -Before.DRI1OT, Week);
                After.StaffCount--;
            }
            if (After.DRI2Id != 0)
            {
                if (Before.DRI2Id == 0)
                {
                    After.StaffCount++;
                    UpdateHours(After.DRI2Id, After.DRI2LOD, After.DRI2UNS, After.DRI2OT, Week);
                }
                else if (Before.DRI2Id == After.DRI2Id)
                {
                    if (Before.DRI2LOD != After.DRI2LOD)
                    {
                        double appointed = After.DRI2LOD - Before.DRI2LOD;
                        UpdateAppointed(After.DRI2Id, appointed, Week);
                    }
                    if (Before.DRI2UNS != After.DRI2UNS)
                    {
                        double unsocial = After.DRI2UNS - Before.DRI2UNS;
                        UpdateUnsocial(After.DRI2Id, unsocial, Week);
                    }
                    if (Before.DRI2OT != After.DRI2OT)
                    {
                        double overtime = After.DRI2OT - Before.DRI2OT;
                        UpdateOvertime(After.DRI2Id, overtime, Week);
                    }
                }
                else if (Before.DRI2Id != After.DRI2Id)
                {
                    UpdateHours(Before.DRI2Id, -Before.DRI2LOD, -Before.DRI2UNS, -Before.DRI2OT, Week);
                    UpdateHours(After.DRI2Id, After.DRI2LOD, After.DRI2UNS, After.DRI2OT, Week);
                }
            }
            else if (After.DRI2Id == 0 && Before.DRI2Id != 0)
            {
                UpdateHours(Before.DRI2Id, -Before.DRI2LOD, -Before.DRI2UNS, -Before.DRI2OT, Week);
                After.StaffCount--;
            }

            //RNs - no staffcount addition
            if (After.RN1Id != 0)
            {
                if (Before.RN1Id == 0)
                {
                    UpdateHours(After.RN1Id, After.RN1LOD, After.RN1UNS, After.RN1OT, Week);
                }
                else if (Before.RN1Id == After.RN1Id)
                {
                    if (Before.RN1LOD != After.RN1LOD)
                    {
                        double appointed = After.RN1LOD - Before.RN1LOD;
                        UpdateAppointed(After.RN1Id, appointed, Week);
                    }
                    if (Before.RN1UNS != After.RN1UNS)
                    {
                        double unsocial = After.RN1UNS - Before.RN1UNS;
                        UpdateUnsocial(After.RN1Id, unsocial, Week);
                    }
                    if (Before.RN1OT != After.RN1OT)
                    {
                        double overtime = After.RN1OT - Before.RN1OT;
                        UpdateOvertime(After.RN1Id, overtime, Week);
                    }
                }
                else if (Before.RN1Id != After.RN1Id)
                {
                    UpdateHours(Before.RN1Id, -Before.RN1LOD, -Before.RN1UNS, -Before.RN1OT, Week);
                    UpdateHours(After.RN1Id, After.RN1LOD, After.RN1UNS, After.RN1OT, Week);
                }
            }
            else if (After.RN1Id == 0 && Before.RN1Id != 0)
            {
                UpdateHours(Before.RN1Id, -Before.RN1LOD, -Before.RN1UNS, -Before.RN1OT, Week);
            }
            if (After.RN2Id != 0)
            {
                if (Before.RN2Id == 0)
                {
                    UpdateHours(After.RN2Id, After.RN2LOD, After.RN2UNS, After.RN2OT, Week);
                }
                else if (Before.RN2Id == After.RN2Id)
                {
                    if (Before.RN2LOD != After.RN2LOD)
                    {
                        double appointed = After.RN2LOD - Before.RN2LOD;
                        UpdateAppointed(After.RN2Id, appointed, Week);
                    }
                    if (Before.RN2UNS != After.RN2UNS)
                    {
                        double unsocial = After.RN2UNS - Before.RN2UNS;
                        UpdateUnsocial(After.RN2Id, unsocial, Week);
                    }
                    if (Before.RN2OT != After.RN2OT)
                    {
                        double overtime = After.RN2OT - Before.RN2OT;
                        UpdateOvertime(After.RN2Id, overtime, Week);
                    }
                }
                else if (Before.RN2Id != After.RN2Id)
                {
                    UpdateHours(Before.RN2Id, -Before.RN2LOD, -Before.RN2UNS, -Before.RN2OT, Week);
                    UpdateHours(After.RN2Id, After.RN2LOD, After.RN2UNS, After.RN2OT, Week);
                }
            }
            else if (After.RN2Id == 0 && Before.RN2Id != 0)
            {
                UpdateHours(Before.RN2Id, -Before.RN2LOD, -Before.RN2UNS, -Before.RN2OT, Week);
            }
            if (After.RN3Id != 0)
            {
                if (Before.RN3Id == 0)
                {
                    UpdateHours(After.RN3Id, After.RN3LOD, After.RN3UNS, After.RN3OT, Week);
                }
                else if (Before.RN3Id == After.RN3Id)
                {
                    if (Before.RN3LOD != After.RN3LOD)
                    {
                        double appointed = After.RN3LOD - Before.RN3LOD;
                        UpdateAppointed(After.RN3Id, appointed, Week);
                    }
                    if (Before.RN3UNS != After.RN3UNS)
                    {
                        double unsocial = After.RN3UNS - Before.RN3UNS;
                        UpdateUnsocial(After.RN3Id, unsocial, Week);
                    }
                    if (Before.RN3OT != After.RN3OT)
                    {
                        double overtime = After.RN3OT - Before.RN3OT;
                        UpdateOvertime(After.RN3Id, overtime, Week);
                    }
                }
                else if (Before.RN3Id != After.RN3Id)
                {
                    UpdateHours(Before.RN3Id, -Before.RN3LOD, -Before.RN3UNS, -Before.RN3OT, Week);
                    UpdateHours(After.RN3Id, After.RN3LOD, After.RN3UNS, After.RN3OT, Week);
                }
            }
            else if (After.RN3Id == 0 && Before.RN3Id != 0)
            {
                UpdateHours(Before.RN3Id, -Before.RN3LOD, -Before.RN3UNS, -Before.RN3OT, Week);
            }

            //CCAs
            if (After.CCA1Id != 0)
            {
                if (Before.CCA1Id == 0)
                {
                    After.StaffCount++;
                    UpdateHours(After.CCA1Id, After.CCA1LOD, After.CCA1UNS, After.CCA1OT, Week);
                }
                else if (Before.CCA1Id == After.CCA1Id)
                {
                    if (Before.CCA1LOD != After.CCA1LOD)
                    {
                        double appointed = After.CCA1LOD - Before.CCA1LOD;
                        UpdateAppointed(After.CCA1Id, appointed, Week);
                    }
                    if (Before.CCA1UNS != After.CCA1UNS)
                    {
                        double unsocial = After.CCA1UNS - Before.CCA1UNS;
                        UpdateUnsocial(After.CCA1Id, unsocial, Week);
                    }
                    if (Before.CCA1OT != After.CCA1OT)
                    {
                        double overtime = After.CCA1OT - Before.CCA1OT;
                        UpdateOvertime(After.CCA1Id, overtime, Week);
                    }
                }
                else if (Before.CCA1Id != After.CCA1Id)
                {
                    UpdateHours(Before.CCA1Id, -Before.CCA1LOD, -Before.CCA1UNS, -Before.CCA1OT, Week);
                    UpdateHours(After.CCA1Id, After.CCA1LOD, After.CCA1UNS, After.CCA1OT, Week);
                }
            }
            else if (After.CCA1Id == 0 && Before.CCA1Id != 0)
            {
                UpdateHours(Before.CCA1Id, -Before.CCA1LOD, -Before.CCA1UNS, -Before.CCA1OT, Week);
                After.StaffCount--;
            }
            if (After.CCA2Id != 0)
            {
                if (Before.CCA2Id == 0)
                {
                    After.StaffCount++;
                    UpdateHours(After.CCA2Id, After.CCA2LOD, After.CCA2UNS, After.CCA2OT, Week);
                }
                else if (Before.CCA2Id == After.CCA2Id)
                {
                    if (Before.CCA2LOD != After.CCA2LOD)
                    {
                        double appointed = After.CCA2LOD - Before.CCA2LOD;
                        UpdateAppointed(After.CCA2Id, appointed, Week);
                    }
                    if (Before.CCA2UNS != After.CCA2UNS)
                    {
                        double unsocial = After.CCA2UNS - Before.CCA2UNS;
                        UpdateUnsocial(After.CCA2Id, unsocial, Week);
                    }
                    if (Before.CCA2OT != After.CCA2OT)
                    {
                        double overtime = After.CCA2OT - Before.CCA2OT;
                        UpdateOvertime(After.CCA2Id, overtime, Week);
                    }
                }
                else if (Before.CCA2Id != After.CCA2Id)
                {
                    UpdateHours(Before.CCA2Id, -Before.CCA2LOD, -Before.CCA2UNS, -Before.CCA2OT, Week);
                    UpdateHours(After.CCA2Id, After.CCA2LOD, After.CCA2UNS, After.CCA2OT, Week);
                }
            }
            else if (After.CCA2Id == 0 && Before.CCA2Id != 0)
            {
                UpdateHours(Before.CCA2Id, -Before.CCA2LOD, -Before.CCA2UNS, -Before.CCA2OT, Week);
                After.StaffCount--;
            }
            if (After.CCA3Id != 0)
            {
                if (Before.CCA3Id == 0)
                {
                    After.StaffCount++;
                    UpdateHours(After.CCA3Id, After.CCA3LOD, After.CCA3UNS, After.CCA3OT, Week);
                }
                else if (Before.CCA3Id == After.CCA3Id)
                {
                    if (Before.CCA3LOD != After.CCA3LOD)
                    {
                        double appointed = After.CCA3LOD - Before.CCA3LOD;
                        UpdateAppointed(After.CCA3Id, appointed, Week);
                    }
                    if (Before.CCA3UNS != After.CCA3UNS)
                    {
                        double unsocial = After.CCA3UNS - Before.CCA3UNS;
                        UpdateUnsocial(After.CCA3Id, unsocial, Week);
                    }
                    if (Before.CCA3OT != After.CCA3OT)
                    {
                        double overtime = After.CCA3OT - Before.CCA3OT;
                        UpdateOvertime(After.CCA3Id, overtime, Week);
                    }
                }
                else if (Before.CCA3Id != After.CCA3Id)
                {
                    UpdateHours(Before.CCA3Id, -Before.CCA3LOD, -Before.CCA3UNS, -Before.CCA3OT, Week);
                    UpdateHours(After.CCA3Id, After.CCA3LOD, After.CCA3UNS, After.CCA3OT, Week);
                }
            }
            else if (After.CCA3Id == 0 && Before.CCA3Id != 0)
            {
                UpdateHours(Before.CCA3Id, -Before.CCA3LOD, -Before.CCA3UNS, -Before.CCA3OT, Week);
                After.StaffCount--;
            }
            System.Diagnostics.Debug.WriteLine(After.StaffCount.ToString());
        }

        public void UpdateRosterLowRate(Session Before, Session After)
        {
            After.StaffCount = 0;
            double Week = GetWeek(After.Date);
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
                After.StaffCount--;
            }

            //Drivers
            if (After.DRI1Id != 0)
            {
                if (Before.DRI1Id == 0)
                {
                    After.StaffCount++;
                    UpdateHoursLowRate(After.DRI1Id, After.DRI1LOD, After.DRI1OT, Week);
                }
                else if (Before.DRI1Id == After.DRI1Id)
                {
                    if (Before.DRI1LOD != After.DRI1LOD)
                    {
                        double appointed = After.DRI1LOD - Before.DRI1LOD;
                        UpdateLowRateAppointed(After.DRI1Id, appointed, Week);
                    }
                    if (Before.DRI1OT != After.DRI1OT)
                    {
                        double overtime = After.DRI1OT - Before.DRI1OT;
                        UpdateOvertime(After.DRI1Id, overtime, Week);
                    }
                }
                else if (Before.DRI1Id != After.DRI1Id)
                {
                    UpdateHoursLowRate(Before.DRI1Id, -Before.DRI1LOD, -Before.DRI1OT, Week);
                    UpdateHoursLowRate(After.DRI1Id, After.DRI1LOD, After.DRI1OT, Week);
                }
            }
            else if (After.DRI1Id == 0 && Before.DRI1Id != 0)
            {
                UpdateHoursLowRate(Before.DRI1Id, -Before.DRI1LOD, -Before.DRI1OT, Week);
                After.StaffCount--;
            }
            if (After.DRI2Id != 0)
            {
                if (Before.DRI2Id == 0)
                {
                    After.StaffCount++;
                    UpdateHoursLowRate(After.DRI2Id, After.DRI2LOD, After.DRI2OT, Week);
                }
                else if (Before.DRI2Id == After.DRI2Id)
                {
                    if (Before.DRI2LOD != After.DRI2LOD)
                    {
                        double appointed = After.DRI2LOD - Before.DRI2LOD;
                        UpdateLowRateAppointed(After.DRI2Id, appointed, Week);
                    }
                    if (Before.DRI2OT != After.DRI2OT)
                    {
                        double overtime = After.DRI2OT - Before.DRI2OT;
                        UpdateOvertime(After.DRI2Id, overtime, Week);
                    }
                }
                else if (Before.DRI2Id != After.DRI2Id)
                {
                    UpdateHoursLowRate(Before.DRI2Id, -Before.DRI2LOD, -Before.DRI2OT, Week);
                    UpdateHoursLowRate(After.DRI2Id, After.DRI2LOD, After.DRI2OT, Week);
                }
            }
            else if (After.DRI2Id == 0 && Before.DRI2Id != 0)
            {
                UpdateHoursLowRate(Before.DRI2Id, -Before.DRI2LOD, -Before.DRI2OT, Week);
                After.StaffCount--;
            }

            //RNs - no staffcount addition
            if (After.RN1Id != 0)
            {
                if (Before.RN1Id == 0)
                {
                    UpdateHoursLowRate(After.RN1Id, After.RN1LOD, After.RN1OT, Week);
                }
                else if (Before.RN1Id == After.RN1Id)
                {
                    if (Before.RN1LOD != After.RN1LOD)
                    {
                        double appointed = After.RN1LOD - Before.RN1LOD;
                        UpdateLowRateAppointed(After.RN1Id, appointed, Week);
                    }
                    if (Before.RN1OT != After.RN1OT)
                    {
                        double overtime = After.RN1OT - Before.RN1OT;
                        UpdateOvertime(After.RN1Id, overtime, Week);
                    }
                }
                else if (Before.RN1Id != After.RN1Id)
                {
                    UpdateHoursLowRate(Before.RN1Id, -Before.RN1LOD, -Before.RN1OT, Week);
                    UpdateHoursLowRate(After.RN1Id, After.RN1LOD, After.RN1OT, Week);
                }
            }
            else if (After.RN1Id == 0 && Before.RN1Id != 0)
            {
                UpdateHoursLowRate(Before.RN1Id, -Before.RN1LOD, -Before.RN1OT, Week);
            }
            if (After.RN2Id != 0)
            {
                if (Before.RN2Id == 0)
                {
                    UpdateHoursLowRate(After.RN2Id, After.RN2LOD, After.RN2OT, Week);
                }
                else if (Before.RN2Id == After.RN2Id)
                {
                    if (Before.RN2LOD != After.RN2LOD)
                    {
                        double appointed = After.RN2LOD - Before.RN2LOD;
                        UpdateLowRateAppointed(After.RN2Id, appointed, Week);
                    }
                    if (Before.RN2OT != After.RN2OT)
                    {
                        double overtime = After.RN2OT - Before.RN2OT;
                        UpdateOvertime(After.RN2Id, overtime, Week);
                    }
                }
                else if (Before.RN2Id != After.RN2Id)
                {
                    UpdateHoursLowRate(Before.RN2Id, -Before.RN2LOD, -Before.RN2OT, Week);
                    UpdateHoursLowRate(After.RN2Id, After.RN2LOD, After.RN2OT, Week);
                }
            }
            else if (After.RN2Id == 0 && Before.RN2Id != 0)
            {
                UpdateHoursLowRate(Before.RN2Id, -Before.RN2LOD, -Before.RN2OT, Week);
            }
            if (After.RN3Id != 0)
            {
                if (Before.RN3Id == 0)
                {
                    UpdateHoursLowRate(After.RN3Id, After.RN3LOD, After.RN3OT, Week);
                }
                else if (Before.RN3Id == After.RN3Id)
                {
                    if (Before.RN3LOD != After.RN3LOD)
                    {
                        double appointed = After.RN3LOD - Before.RN3LOD;
                        UpdateLowRateAppointed(After.RN3Id, appointed, Week);
                    }
                    if (Before.RN3OT != After.RN3OT)
                    {
                        double overtime = After.RN3OT - Before.RN3OT;
                        UpdateOvertime(After.RN3Id, overtime, Week);
                    }
                }
                else if (Before.RN3Id != After.RN3Id)
                {
                    UpdateHoursLowRate(Before.RN3Id, -Before.RN3LOD, -Before.RN3OT, Week);
                    UpdateHoursLowRate(After.RN3Id, After.RN3LOD, After.RN3OT, Week);
                }
            }
            else if (After.RN3Id == 0 && Before.RN3Id != 0)
            {
                UpdateHoursLowRate(Before.RN3Id, -Before.RN3LOD, -Before.RN3OT, Week);
            }
            //CCAs
            if (After.CCA1Id != 0)
            {
                if (Before.CCA1Id == 0)
                {
                    After.StaffCount++;
                    UpdateHoursLowRate(After.CCA1Id, After.CCA1LOD, After.CCA1OT, Week);
                }
                else if (Before.CCA1Id == After.CCA1Id)
                {
                    if (Before.CCA1LOD != After.CCA1LOD)
                    {
                        double appointed = After.CCA1LOD - Before.CCA1LOD;
                        UpdateLowRateAppointed(After.CCA1Id, appointed, Week);
                    }
                    if (Before.CCA1OT != After.CCA1OT)
                    {
                        double overtime = After.CCA1OT - Before.CCA1OT;
                        UpdateOvertime(After.CCA1Id, overtime, Week);
                    }
                }
                else if (Before.CCA1Id != After.CCA1Id)
                {
                    UpdateHoursLowRate(Before.CCA1Id, -Before.CCA1LOD, -Before.CCA1OT, Week);
                    UpdateHoursLowRate(After.CCA1Id, After.CCA1LOD, After.CCA1OT, Week);
                }
            }
            else if (After.CCA1Id == 0 && Before.CCA1Id != 0)
            {
                UpdateHoursLowRate(Before.CCA1Id, -Before.CCA1LOD, -Before.CCA1OT, Week);
                After.StaffCount--;
            }
            if (After.CCA2Id != 0)
            {
                if (Before.CCA2Id == 0)
                {
                    After.StaffCount++;
                    UpdateHoursLowRate(After.CCA2Id, After.CCA2LOD, After.CCA2OT, Week);
                }
                else if (Before.CCA2Id == After.CCA2Id)
                {
                    if (Before.CCA2LOD != After.CCA2LOD)
                    {
                        double appointed = After.CCA2LOD - Before.CCA2LOD;
                        UpdateLowRateAppointed(After.CCA2Id, appointed, Week);
                    }
                    if (Before.CCA2OT != After.CCA2OT)
                    {
                        double overtime = After.CCA2OT - Before.CCA2OT;
                        UpdateOvertime(After.CCA2Id, overtime, Week);
                    }
                }
                else if (Before.CCA2Id != After.CCA2Id)
                {
                    UpdateHoursLowRate(Before.CCA2Id, -Before.CCA2LOD, -Before.CCA2OT, Week);
                    UpdateHoursLowRate(After.CCA2Id, After.CCA2LOD, After.CCA2OT, Week);
                }
            }
            else if (After.CCA2Id == 0 && Before.CCA2Id != 0)
            {
                UpdateHoursLowRate(Before.CCA2Id, -Before.CCA2LOD, -Before.CCA2OT, Week);
                After.StaffCount--;
            }
            if (After.CCA3Id != 0)
            {
                if (Before.CCA3Id == 0)
                {
                    After.StaffCount++;
                    UpdateHoursLowRate(After.CCA3Id, After.CCA3LOD, After.CCA3OT, Week);
                }
                else if (Before.CCA3Id == After.CCA3Id)
                {
                    if (Before.CCA3LOD != After.CCA3LOD)
                    {
                        double appointed = After.CCA3LOD - Before.CCA3LOD;
                        UpdateLowRateAppointed(After.CCA3Id, appointed, Week);
                    }
                    if (Before.CCA3OT != After.CCA3OT)
                    {
                        double overtime = After.CCA3OT - Before.CCA3OT;
                        UpdateOvertime(After.CCA3Id, overtime, Week);
                    }
                }
                else if (Before.CCA3Id != After.CCA3Id)
                {
                    UpdateHoursLowRate(Before.CCA3Id, -Before.CCA3LOD, -Before.CCA3OT, Week);
                    UpdateHoursLowRate(After.CCA3Id, After.CCA3LOD, After.CCA3OT, Week);
                }
            }
            else if (After.CCA3Id == 0 && Before.CCA3Id != 0)
            {
                UpdateHoursLowRate(Before.CCA3Id, -Before.CCA3LOD, -Before.CCA3OT, Week);
                After.StaffCount--;
            }
        }

        public void UpdateRosterHighRate(Session Before, Session After)
        {
            After.StaffCount = 0;
            double Week = GetWeek(After.Date);
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
                After.StaffCount--;
            }

            //Drivers
            if (After.DRI1Id != 0)
            {
                if (Before.DRI1Id == 0)
                {
                    After.StaffCount++;
                    UpdateHoursHighRate(After.DRI1Id, After.DRI1LOD, After.DRI1OT, Week);
                }
                else if (Before.DRI1Id == After.DRI1Id)
                {
                    if (Before.DRI1LOD != After.DRI1LOD)
                    {
                        double appointed = After.DRI1LOD - Before.DRI1LOD;
                        UpdateHighRateAppointed(After.DRI1Id, appointed, Week);
                    }
                    if (Before.DRI1OT != After.DRI1OT)
                    {
                        double overtime = After.DRI1OT - Before.DRI1OT;
                        UpdateOvertime(After.DRI1Id, overtime, Week);
                    }
                }
                else if (Before.DRI1Id != After.DRI1Id)
                {
                    UpdateHoursHighRate(Before.DRI1Id, -Before.DRI1LOD, -Before.DRI1OT, Week);
                    UpdateHoursHighRate(After.DRI1Id, After.DRI1LOD, After.DRI1OT, Week);
                }
            }
            else if (After.DRI1Id == 0 && Before.DRI1Id != 0)
            {
                UpdateHoursHighRate(Before.DRI1Id, -Before.DRI1LOD, -Before.DRI1OT, Week);
                After.StaffCount--;
            }
            if (After.DRI2Id != 0)
            {
                if (Before.DRI2Id == 0)
                {
                    After.StaffCount++;
                    UpdateHoursHighRate(After.DRI2Id, After.DRI2LOD, After.DRI2OT, Week);
                }
                else if (Before.DRI2Id == After.DRI2Id)
                {
                    if (Before.DRI2LOD != After.DRI2LOD)
                    {
                        double appointed = After.DRI2LOD - Before.DRI2LOD;
                        UpdateHighRateAppointed(After.DRI2Id, appointed, Week);
                    }
                    if (Before.DRI2OT != After.DRI2OT)
                    {
                        double overtime = After.DRI2OT - Before.DRI2OT;
                        UpdateOvertime(After.DRI2Id, overtime, Week);
                    }
                }
                else if (Before.DRI2Id != After.DRI2Id)
                {
                    UpdateHoursHighRate(Before.DRI2Id, -Before.DRI2LOD, -Before.DRI2OT, Week);
                    UpdateHoursHighRate(After.DRI2Id, After.DRI2LOD, After.DRI2OT, Week);
                }
            }
            else if (After.DRI2Id == 0 && Before.DRI2Id != 0)
            {
                UpdateHoursHighRate(Before.DRI2Id, -Before.DRI2LOD, -Before.DRI2OT, Week);
                After.StaffCount--;
            }

            //RNs - no staffcount addition
            if (After.RN1Id != 0)
            {
                if (Before.RN1Id == 0)
                {
                    UpdateHoursHighRate(After.RN1Id, After.RN1LOD, After.RN1OT, Week);
                }
                else if (Before.RN1Id == After.RN1Id)
                {
                    if (Before.RN1LOD != After.RN1LOD)
                    {
                        double appointed = After.RN1LOD - Before.RN1LOD;
                        UpdateHighRateAppointed(After.RN1Id, appointed, Week);
                    }
                    if (Before.RN1OT != After.RN1OT)
                    {
                        double overtime = After.RN1OT - Before.RN1OT;
                        UpdateOvertime(After.RN1Id, overtime, Week);
                    }
                }
                else if (Before.RN1Id != After.RN1Id)
                {
                    UpdateHoursHighRate(Before.RN1Id, -Before.RN1LOD, -Before.RN1OT, Week);
                    UpdateHoursHighRate(After.RN1Id, After.RN1LOD, After.RN1OT, Week);
                }
            }
            else if (After.RN1Id == 0 && Before.RN1Id != 0)
            {
                UpdateHoursHighRate(Before.RN1Id, -Before.RN1LOD, -Before.RN1OT, Week);
            }
            if (After.RN2Id != 0)
            {
                if (Before.RN2Id == 0)
                {
                    UpdateHoursHighRate(After.RN2Id, After.RN2LOD, After.RN2OT, Week);
                }
                else if (Before.RN2Id == After.RN2Id)
                {
                    if (Before.RN2LOD != After.RN2LOD)
                    {
                        double appointed = After.RN2LOD - Before.RN2LOD;
                        UpdateHighRateAppointed(After.RN2Id, appointed, Week);
                    }
                    if (Before.RN2OT != After.RN2OT)
                    {
                        double overtime = After.RN2OT - Before.RN2OT;
                        UpdateOvertime(After.RN2Id, overtime, Week);
                    }
                }
                else if (Before.RN2Id != After.RN2Id)
                {
                    UpdateHoursHighRate(Before.RN2Id, -Before.RN2LOD, -Before.RN2OT, Week);
                    UpdateHoursHighRate(After.RN2Id, After.RN2LOD, After.RN2OT, Week);
                }
            }
            else if (After.RN2Id == 0 && Before.RN2Id != 0)
            {
                UpdateHoursHighRate(Before.RN2Id, -Before.RN2LOD, -Before.RN2OT, Week);
            }
            if (After.RN3Id != 0)
            {
                if (Before.RN3Id == 0)
                {
                    UpdateHoursHighRate(After.RN3Id, After.RN3LOD, After.RN3OT, Week);
                }
                else if (Before.RN3Id == After.RN3Id)
                {
                    if (Before.RN3LOD != After.RN3LOD)
                    {
                        double appointed = After.RN3LOD - Before.RN3LOD;
                        UpdateHighRateAppointed(After.RN3Id, appointed, Week);
                    }
                    if (Before.RN3OT != After.RN3OT)
                    {
                        double overtime = After.RN3OT - Before.RN3OT;
                        UpdateOvertime(After.RN3Id, overtime, Week);
                    }
                }
                else if (Before.RN3Id != After.RN3Id)
                {
                    UpdateHoursHighRate(Before.RN3Id, -Before.RN3LOD, -Before.RN3OT, Week);
                    UpdateHoursHighRate(After.RN3Id, After.RN3LOD, After.RN3OT, Week);
                }
            }
            else if (After.RN3Id == 0 && Before.RN3Id != 0)
            {
                UpdateHoursHighRate(Before.RN3Id, -Before.RN3LOD, -Before.RN3OT, Week);
            }
            //CCAs
            if (After.CCA1Id != 0)
            {
                if (Before.CCA1Id == 0)
                {
                    After.StaffCount++;
                    UpdateHoursHighRate(After.CCA1Id, After.CCA1LOD, After.CCA1OT, Week);
                }
                else if (Before.CCA1Id == After.CCA1Id)
                {
                    if (Before.CCA1LOD != After.CCA1LOD)
                    {
                        double appointed = After.CCA1LOD - Before.CCA1LOD;
                        UpdateHighRateAppointed(After.CCA1Id, appointed, Week);
                    }
                    if (Before.CCA1OT != After.CCA1OT)
                    {
                        double overtime = After.CCA1OT - Before.CCA1OT;
                        UpdateOvertime(After.CCA1Id, overtime, Week);
                    }
                }
                else if (Before.CCA1Id != After.CCA1Id)
                {
                    UpdateHoursHighRate(Before.CCA1Id, -Before.CCA1LOD, -Before.CCA1OT, Week);
                    UpdateHoursHighRate(After.CCA1Id, After.CCA1LOD, After.CCA1OT, Week);
                }
            }
            else if (After.CCA1Id == 0 && Before.CCA1Id != 0)
            {
                UpdateHoursHighRate(Before.CCA1Id, -Before.CCA1LOD, -Before.CCA1OT, Week);
                After.StaffCount--;
            }
            if (After.CCA2Id != 0)
            {
                if (Before.CCA2Id == 0)
                {
                    After.StaffCount++;
                    UpdateHoursHighRate(After.CCA2Id, After.CCA2LOD, After.CCA2OT, Week);
                }
                else if (Before.CCA2Id == After.CCA2Id)
                {
                    if (Before.CCA2LOD != After.CCA2LOD)
                    {
                        double appointed = After.CCA2LOD - Before.CCA2LOD;
                        UpdateHighRateAppointed(After.CCA2Id, appointed, Week);
                    }
                    if (Before.CCA2OT != After.CCA2OT)
                    {
                        double overtime = After.CCA2OT - Before.CCA2OT;
                        UpdateOvertime(After.CCA2Id, overtime, Week);
                    }
                }
                else if (Before.CCA2Id != After.CCA2Id)
                {
                    UpdateHoursHighRate(Before.CCA2Id, -Before.CCA2LOD, -Before.CCA2OT, Week);
                    UpdateHoursHighRate(After.CCA2Id, After.CCA2LOD, After.CCA2OT, Week);
                }
            }
            else if (After.CCA2Id == 0 && Before.CCA2Id != 0)
            {
                UpdateHoursHighRate(Before.CCA2Id, -Before.CCA2LOD, -Before.CCA2OT, Week);
                After.StaffCount--;
            }
            if (After.CCA3Id != 0)
            {
                if (Before.CCA3Id == 0)
                {
                    After.StaffCount++;
                    UpdateHoursHighRate(After.CCA3Id, After.CCA3LOD, After.CCA3OT, Week);
                }
                else if (Before.CCA3Id == After.CCA3Id)
                {
                    if (Before.CCA3LOD != After.CCA3LOD)
                    {
                        double appointed = After.CCA3LOD - Before.CCA3LOD;
                        UpdateHighRateAppointed(After.CCA3Id, appointed, Week);
                    }
                    if (Before.CCA3OT != After.CCA3OT)
                    {
                        double overtime = After.CCA3OT - Before.CCA3OT;
                        UpdateOvertime(After.CCA3Id, overtime, Week);
                    }
                }
                else if (Before.CCA3Id != After.CCA3Id)
                {
                    UpdateHoursHighRate(Before.CCA3Id, -Before.CCA3LOD, -Before.CCA3OT, Week);
                    UpdateHoursHighRate(After.CCA3Id, After.CCA3LOD, After.CCA3OT, Week);
                }
            }
            else if (After.CCA3Id == 0 && Before.CCA3Id != 0)
            {
                UpdateHoursHighRate(Before.CCA3Id, -Before.CCA3LOD, -Before.CCA3OT, Week);
                After.StaffCount--;
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
