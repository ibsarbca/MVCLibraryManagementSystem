using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using MVCLibraryManagementSystem.Models;

namespace MVCLibraryManagementSystem.DAL
{
    public class BookService : IBookService
    {
        private LibraryContext dbcontext;

        public BookService(LibraryContext ctx)
        {
            dbcontext = ctx;
        }

        public IEnumerable<Book> GetAllBooks()
        {
            return dbcontext.Books.ToList();
        }

        public Book GetBookById(int? id)
        {
            return dbcontext.Books.Find(id);
        }

        public void Add(Book b)
        {
            dbcontext.Books.Add(b);
            dbcontext.SaveChanges();
        }

        public void Update(Book b)
        {
            dbcontext.Entry(b).State = EntityState.Modified;
            dbcontext.SaveChanges();
        }

        public void Delete(int id)
        {
            dbcontext.Entry(dbcontext.Books.Find(id)).State = EntityState.Deleted;
            dbcontext.SaveChanges();
        }


        public void Dispose()
        {
            dbcontext.Dispose();
        }
    }
}