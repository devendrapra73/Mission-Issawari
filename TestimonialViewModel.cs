using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace missFinal.Models
{
    public class TestimonialViewModel
    {
        public List<Tbl_Story> Stories { get; set; }
        public Tbl_Story NewStory { get; set; } // Optional, if you're using this to bind the form too
    }
}