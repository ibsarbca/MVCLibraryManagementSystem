using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MVCLibraryManagementSystem.Models
{
    public class AccessionRecord
    {
        public int AccessionRecordId { get; set; }
        [Required]
        public virtual Item Item { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateOfReceipt { get; set; }
        public string Source { get; set; }
        public int Price { get; set; }
    }
}