using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCLibraryManagementSystem.DAL
{
    public class ItemService : IItemService
    {
        private LibraryContext dbcontext;

        public ItemService(LibraryContext ctx)
        {
            dbcontext = ctx;
        }

        public IEnumerable<Models.Item> GetAllItems()
        {
            return dbcontext.Items.ToList();
        }

        public Models.Item GetItemById(int? id)
        {
            return dbcontext.Items.Find(id);
        }

        public void Add(Models.Item i)
        {
            dbcontext.Items.Add(i);
            dbcontext.SaveChanges();
        }

        public void Update(Models.Item i)
        {
            dbcontext.Entry(i).State = System.Data.Entity.EntityState.Modified;
            dbcontext.SaveChanges();
        }

        public void Delete(int? id)
        {
            dbcontext.Entry(this.GetItemById(id)).State = System.Data.Entity.EntityState.Modified;
            dbcontext.SaveChanges();
        }

        public Models.Book GetBookByItemId(int? id)
        {
            return dbcontext.Books.Where(b => b.Item.ItemId == id).First();
        }

        public Models.Newspaper GetNewspaperByItemId(int? id)
        {
            return dbcontext.Newspapers.Where(b => b.Item.ItemId == id).First();
        }

        public Models.QuestionPaper GetQuestionPaperByItemId(int? id)
        {
            return dbcontext.QuestionPapers.Where(b => b.Item.ItemId == id).First();
        }

        public void Dispose()
        {
            dbcontext.Dispose();
        }

    }
}