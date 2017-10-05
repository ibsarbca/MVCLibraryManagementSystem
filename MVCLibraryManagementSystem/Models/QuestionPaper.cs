using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCLibraryManagementSystem.Models
{
    public enum DEPARTMENTS
    {
        BCA,
        BBA,
        MBA
    };
    public class QuestionPaper
    {
        public int QuestionPaperId { get; set; }
        public virtual Item Item { get; set; }
        public DEPARTMENTS Department { get; set; }
        public int Month { get; set; }
        public string Subject { get; set; }
    }
}