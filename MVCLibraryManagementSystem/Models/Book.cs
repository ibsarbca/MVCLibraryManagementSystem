using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MVCLibraryManagementSystem.Models
{
    public enum BOOKTYPES
    {
        TEXTBOOK,
        NON_FICTION,
        FICTION,
        OTHER
    };

    public class Book
    {
        public int BookId { get; set; }
        public virtual Item Item { get; set; }
        public string Author { get; set; }
        public BOOKTYPES BookType { get; set; }
    }
}