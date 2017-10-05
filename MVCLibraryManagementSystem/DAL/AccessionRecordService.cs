using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVCLibraryManagementSystem.Models;

namespace MVCLibraryManagementSystem.DAL
{
    public class AccessionRecordService: IAccessionRecordService
    {
        private LibraryContext dbcontext;
        private IItemService itemService;


        public AccessionRecordService()
        {
            dbcontext = new LibraryContext();
            itemService = new ItemService(dbcontext);
        }

        public AccessionRecordService(LibraryContext ctx, IItemService _iserv)
        {
            dbcontext = ctx;
            this.itemService = _iserv;
        }

        public IEnumerable<Models.AccessionRecord> GetAllAccessionRecords()
        {
            return dbcontext.AccessionRecords.ToList() ;
        }

        public IItemService GetItemService()
        {
            return itemService;
        }

        public Models.AccessionRecord GetAccessionRecordById(int? id)
        {
            return dbcontext.AccessionRecords.Find(id);
        }

        public void Add(Models.AccessionRecord r)
        {
            dbcontext.AccessionRecords.Add(r);
            dbcontext.SaveChanges();
        }

        public void Update(Models.AccessionRecord b)
        {
            dbcontext.Entry(b).State = System.Data.Entity.EntityState.Modified;
            dbcontext.SaveChanges();
        }

        public void Delete(int id)
        {
            dbcontext.Entry(dbcontext.AccessionRecords.Find(id)).State = System.Data.Entity.EntityState.Deleted;
            dbcontext.SaveChanges();
        }

        public void Dispose()
        {
            dbcontext.Dispose();
        }
    }
}