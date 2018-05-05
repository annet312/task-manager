using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskMng.Models
{
    public class TeamView
    {
        public int Id { get; set; }
        public string ManagerName;
        public string TeamName { get; set; }
    }
}