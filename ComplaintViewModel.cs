using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Miss.Models
{
    public class ComplaintViewModel
    {
        public string Complaint_no { get; set; }
        public string Full_Name { get; set; }
        public string Complaint_Type_Name { get; set; } // ✅ Complaint type as name instead of ID
        public string Complaint_Desp { get; set; }
        public string U_Location { get; set; }
        public string Evidence { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }  // ✅ Ensure this property exists
    }


}