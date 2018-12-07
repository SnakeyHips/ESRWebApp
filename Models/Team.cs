using System.Collections.Generic;

namespace ERSWebApp.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<TeamMember> Members { get; set; }
    }
}
