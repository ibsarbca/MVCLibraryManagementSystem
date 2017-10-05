using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCLibraryManagementSystem.Models
{
    public enum CATEGORIES
    {
        SPORTS,
        FINANCE,
        GENERAL,
        ENTERTAINMENT
    }

    public class Newspaper
    {
        public int NewspaperId { get; set; }
        public virtual Item Item { get; set; }
        public DateTime Date { get; set; }
        public CATEGORIES Category { get; set; }
    }
}