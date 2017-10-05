using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCLibraryManagementSystem.Models;

namespace MVCLibraryManagementSystem.DAL
{
    public interface IBookService : IDisposable
    {
        IEnumerable<Book> GetAllBooks();
        Book GetBookById(int? id);
        void Add(Book b);
        void Update(Book b);
        void Delete(int id);
    }
}
