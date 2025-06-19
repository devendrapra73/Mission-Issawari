using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace missFinal.Models
{
    public class YourDbContext
    {
        public DbSet<tbl_complaint> tbl_complaint { get; set; }
        public DbSet<tbl_complaint_type> tbl_complaint_type { get; set; }
        public DbSet<Department> Department { get; set; } // Department table
    }
}