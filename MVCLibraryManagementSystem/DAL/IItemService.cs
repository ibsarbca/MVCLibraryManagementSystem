using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCLibraryManagementSystem.Models;

namespace MVCLibraryManagementSystem.DAL
{
    public interface IItemService: IDisposable
    {
        IEnumerable<Item> GetAllItems();
        Item GetItemById(int? id);
        Book GetBookByItemId(int? id);
        Newspaper GetNewspaperByItemId(int? id);
        QuestionPaper GetQuestionPaperByItemId(int? id);
        void Add(Item i);
        void Update(Item i);
        void Delete(int? id);
    }
}
