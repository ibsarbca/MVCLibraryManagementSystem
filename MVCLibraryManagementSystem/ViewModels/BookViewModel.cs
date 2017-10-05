using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVCLibraryManagementSystem.Models;
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace MVCLibraryManagementSystem.ViewModels
{
    public class BookViewModel
    {
        [Display(Name = "Title")]
        [Required]
        public string ItemTitle { get; set; }
        [Required]
        public string Author { get; set; }

        [Display(Name = "Year")]
        public int ItemYear { get; set; }
        public BOOKTYPES BookType { get; set; }


    }
}