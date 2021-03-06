//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MCAWebsiteMVC.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class CiaDetails
    {
        public int cia_id { get; set; }

        public string stud_id { get; set; }

        public string sub_id { get; set; }

        [Range(0, 15, ErrorMessage = "Mark should be less than 15")]
        [DisplayName("CIA 1")]
        public int cia1 { get; set; }

        [Range(0, 15, ErrorMessage = "Mark should be less than 15")]
        [DisplayName("CIA 2")]
        public int cia2 { get; set; }

        [DisplayName("Assignment Marks")]
        public int assign_marks { get; set; }

        [DisplayName("Total")]
        public int total { get; set; }
    
        public virtual Student Student { get; set; }
        public virtual Subject Subject { get; set; }
    }
}
