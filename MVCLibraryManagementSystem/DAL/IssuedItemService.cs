using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVCLibraryManagementSystem.Models;
using System.Diagnostics;

namespace MVCLibraryManagementSystem.DAL
{
    public class IssuedItemService: IIssuedItemService
    {
        private LibraryContext dbcontext = new LibraryContext();
        private IAccessionRecordService accRecordService;
        private IMemberService memberService;

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

        public AccessionRecord GetRandomIssuableAccRecord(int itemid)
        {
            IEnumerable<AccessionRecord> list = this.GetAllIssuableAccRecords();

            int maxRecords = list.Count() - 1;
            Random rnd = new Random();
            int randomIndex = rnd.Next(0, maxRecords);
            AccessionRecord retval = list.ElementAt(randomIndex);
            return retval;
        }

        public DateTime GetDueDate(IssuedItem issuedItem)
        {
            DateTime retval = DateTime.Now;

            if(issuedItem.Member.MemberType == MEMBERTYPE.FACULTY)
            {
                retval = issuedItem.IssueDate.AddDays(90).Date;
            }
            else if(issuedItem.Member.MemberType == MEMBERTYPE.STUDENT)
            {
                retval = issuedItem.IssueDate.AddDays(7).Date;
            }
            return retval;
        }

        public int GetLateFee(IssuedItem issuedItem)
        {
            DateTime currentDate = DateTime.Now.Date;

            DateTime dueDate = GetDueDate(issuedItem);
            
            int i = currentDate.Subtract(dueDate).Days;

            Debug.WriteLine(GetDueDate(issuedItem).ToString("dd/MM/yyyy"));

            return i * issuedItem.LateFeePerDay;
        }

        public void Dispose()
        {
            dbcontext.Dispose();
        }
    }
}