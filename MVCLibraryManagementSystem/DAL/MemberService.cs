using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCLibraryManagementSystem.DAL
{
    public class MemberService : IMemberService
    {
        private LibraryContext dbcontext = new LibraryContext();

        public MemberService()
        {
            this.dbcontext = new LibraryContext();
        }

        public MemberService(LibraryContext ctx)
        {
            this.dbcontext = ctx;
        }

        public List<Models.Member> GetAllMembers()
        {
            return dbcontext.Members.ToList();
        }

        public Models.Member GetMemberById(int? memberid)
        {
            return dbcontext.Members.Find(memberid);
        }

        public void Add(Models.Member m)
        {
            dbcontext.Members.Add(m);
            dbcontext.SaveChanges();
        }

        public void Update(Models.Member m)
        {
            dbcontext.Entry(m).State = System.Data.Entity.EntityState.Modified;
            dbcontext.SaveChanges();
        }

        public void Delete(int id)
        {
            dbcontext.Entry(this.GetMemberById(id)).State = System.Data.Entity.EntityState.Deleted;
            dbcontext.SaveChanges();
        }

        public void Dispose()
        {
            dbcontext.Dispose();
        }
    }
}