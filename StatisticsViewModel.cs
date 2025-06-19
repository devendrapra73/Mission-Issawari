using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Miss.Models
{
    public class StatisticsViewModel
    {
        public int TotalComplaintsFiled { get; set; }
        public int CasesResolved { get; set; }
        public int OngoingCases { get; set; }
        public int WomenRescued { get; set; }
        public int HelplineCallsReceived { get; set; }
        public int AwarenessProgramsConducted { get; set; }
    }

}