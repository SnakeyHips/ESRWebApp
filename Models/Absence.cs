namespace ERSWebApp.Models
{
    public class Absence
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public string Type { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public double Hours { get; set; }
    }
}
