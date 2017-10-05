using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MVCLibraryManagementSystem.Models
{
    public class Item
    {
        public int ItemId { get; set; }
        [Required]
        public string Title { get; set; }
        public int Year { get; set; }
    }
}