using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MCAWebsiteMVC.Models
{
    public class ExamViewModel
    {
        public int SemId { get; set; }

        public string subId { get; set; }

        public string studId { get; set; }

        [DisplayName("CIA 1")]
        public int cia1 { get; set; }

        [DisplayName("CIA 2")]
        public int cia2 { get; set; }

        [DisplayName("Assignment Marks")]
        public int assign_marks { get; set; }

        [DisplayName("Total")]
        public int total { get; set; }
    }
}