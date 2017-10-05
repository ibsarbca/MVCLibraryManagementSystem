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

        public IssuedItemsController(IIssuedItemService _service)
        {
            service = _service;
        }

        // GET: IssuedItems
        public ActionResult Index()
        {
            return View(db.IssuedItems.ToList());
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
        public ActionResult Create()
        {
            return View();
        }

        // POST: IssuedItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IssuedItemId,IssueDate,LateFeePerDay")] IssuedItem issuedItem)
        {
            if (ModelState.IsValid)
            {
                db.IssuedItems.Add(issuedItem);
                db.SaveChanges();
                return RedirectToAction("Index");
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
