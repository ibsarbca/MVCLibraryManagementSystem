using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVCLibraryManagementSystem.Models;

namespace MVCLibraryManagementSystem.DAL
{
    public class IssuedItemService: IIssuedItemService
    {
        private LibraryContext dbcontext = new LibraryContext();
        private IAccessionRecordService accRecordService;

        public IssuedItemService()
        {
            ItemService itemService = new ItemService(dbcontext);
            this.accRecordService = new AccessionRecordService(dbcontext, itemService);
        }

        public IssuedItemService(IAccessionRecordService _service)
        {
            this.accRecordService = _service;
        }

        public IssuedItemService(LibraryContext ctx)
        {
            this.dbcontext = ctx;
            ItemService itemService = new ItemService(dbcontext);
            this.accRecordService = new AccessionRecordService(dbcontext, itemService);
        }


        public List<Models.IssuedItem> GetAllIssuedItems()
        {

            /*
             * Get All "Issued Items", returns all items in the Issued Item Table.
             * Shouldn't it return only the items that ARE issued?
             */
            return dbcontext.IssuedItems.ToList();    
        }

        public void Add(Models.IssuedItem i)
        {
            dbcontext.IssuedItems.Add(i);
            dbcontext.SaveChanges();
        }

        public void Update(Models.IssuedItem i)
        {
            dbcontext.Entry(i).State = System.Data.Entity.EntityState.Modified;
            dbcontext.SaveChanges();
        }

        public void Delete(int id)
        {

            dbcontext.Entry(dbcontext.IssuedItems.Find(id)).State = System.Data.Entity.EntityState.Modified;
            dbcontext.SaveChanges();
        }

        public Models.IssuedItem GetById(int? id)
        {
            throw new NotImplementedException();
            
        }


        public IEnumerable<Models.AccessionRecord> GetAllIssuableAccRecords()
        {
            IEnumerable<AccessionRecord> accRecords = accRecordService.GetAllAccessionRecords();

            // Get all the acc. records which not returned
            IEnumerable<AccessionRecord> issuedRecords = this.GetAllIssuedItems().Where(i => i.IsReturned == false).Select(i => i.AccessionRecord);

            var retval = accRecords.Except(issuedRecords);

            return retval; 
        }

        public void Dispose()
        {
            dbcontext.Dispose();
        }
    }
}