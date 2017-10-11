using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCLibraryManagementSystem.DAL;
using MVCLibraryManagementSystem.Models;

namespace MVCLibraryManagementSystem.Controllers
{
    public class IssuedItemsController : Controller
    {
        private LibraryContext db = new LibraryContext();

        public IIssuedItemService service;
        public IMemberService memberService;

        public IssuedItemsController()
        {
            service = new IssuedItemService();
            memberService = new MemberService(db);
        }
        public IssuedItemsController(IIssuedItemService _service)
        {
            service = _service;
        }

        public IssuedItemsController(IIssuedItemService _service, IMemberService _mservice)
        {
            service = _service;
            memberService = _mservice;
        }


        // GET: IssuedItems
        //public ActionResult Index()
        // Index(page: 3)
        // Index(searchString: "")
        public ActionResult Index(String searchString = "", int? page = 1)
        {
            var issuedItems = service.GetAllIssuedItems();

            //Search code starts
            if (!String.IsNullOrEmpty(searchString))
            {
                issuedItems = issuedItems
                              .Where(i => i.AccessionRecord.Item.Title.Contains(searchString))
                              .ToList();
            }
            //Search code ends


            //Pagination code starts
            if (page == null)
            {
                page = 1;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            issuedItems = issuedItems.OrderBy(i => i.IssuedItemId).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            //Pagination code ends

            return View(issuedItems);
        }

        // GET: IssuedItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IssuedItem issuedItem = db.IssuedItems.Find(id);
            if (issuedItem == null)
            {
                return HttpNotFound();
            }
            return View(issuedItem);
        }

        // GET: IssuedItems/Create
        public ActionResult Create(int itemid)
        {
            IssuedItem newRecord = new IssuedItem();
            newRecord.AccessionRecord = service.GetRandomIssuableAccRecord(itemid);
            newRecord.IssueDate = DateTime.Now.Date;
            newRecord.IsReturned = false;
            return View(newRecord);
        }

        // POST: IssuedItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IssuedItemId,IssueDate,LateFeePerDay,Member")] IssuedItem issuedItem)
        {
            if (ModelState.IsValid)
            {
                if(memberService.GetMemberById(issuedItem.Member.MemberId) == null)
                {
                    ModelState.AddModelError("Member", "The Member ID does not exist.");
                } else
                {
                    service.Add(issuedItem);
                    return RedirectToAction("Index");
                }
            }

            return View(issuedItem);
        }

        // GET: IssuedItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IssuedItem issuedItem = db.IssuedItems.Find(id);
            if (issuedItem == null)
            {
                return HttpNotFound();
            }
            return View(issuedItem);
        }

        // POST: IssuedItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IssuedItemId,IssueDate,LateFeePerDay")] IssuedItem issuedItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(issuedItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(issuedItem);
        }

        // GET: IssuedItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IssuedItem issuedItem = db.IssuedItems.Find(id);
            if (issuedItem == null)
            {
                return HttpNotFound();
            }
            return View(issuedItem);
        }

        // POST: IssuedItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IssuedItem issuedItem = db.IssuedItems.Find(id);
            db.IssuedItems.Remove(issuedItem);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetReturned()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetDateReturned()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
