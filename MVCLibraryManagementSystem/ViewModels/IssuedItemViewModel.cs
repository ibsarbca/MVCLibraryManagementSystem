using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVCLibraryManagementSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace MVCLibraryManagementSystem.ViewModels
{
    public class IssuedItemViewModel
    {
        public int AccessionId {get;set;}
        [Required]
        public virtual AccessionRecord AccessionRecord { get; set; }
        [Required]
        public virtual Member Member { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime IssueDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateReturned { get; set; }
        public int LateFeePerDay { get; set; }
        public bool IsReturned { get; set; }
        public bool? IsLateFeePaid { get; set; }


        public IssuedItem()
        {
            IssueDate = DateTime.Now.Date;
            IsReturned = false;
            IsLateFeePaid = null;
        }
    }
}