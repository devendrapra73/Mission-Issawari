using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using missFinal.Models;

namespace missFinal.ViewModels
{
    public class TrainingViewModel
    {
        public List<GovScheme> GovSchemes { get; set; }
        public List<Tbl_Program> Programs { get; set; }
        public List<Tbl_Story> Stories { get; set; }
    }
}