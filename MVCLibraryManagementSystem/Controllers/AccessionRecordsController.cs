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
    public class AccessionRecordsController : Controller
    {
        private IAccessionRecordService service;
        public AccessionRecordsController()
        {
            this.service = new AccessionRecordService();
        }

        public AccessionRecordsController(IAccessionRecordService _serv)
        {
            this.service = _serv;
        }

        // GET: AccessionRecords
        public ActionResult Index()
        {
            return View(service.GetAllAccessionRecords().ToList());
        }

        // GET: AccessionRecords/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccessionRecord accessionRecord = service.GetAccessionRecordById(id);
            if (accessionRecord == null)
            {
                return HttpNotFound();
            }
            return View(accessionRecord);
        }

        // GET: AccessionRecords/Create
        public ActionResult Create(int? itemid)
        {
            AccessionRecord newRecord = new AccessionRecord();
            if(itemid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = service.GetItemService().GetItemById(itemid);
            newRecord.Item = item;
            newRecord.DateOfReceipt = DateTime.Now.Date;
            return View(newRecord);
        }

        // POST: AccessionRecords/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]  
        public ActionResult Create(AccessionRecord accessionRecord)
        {
            
            if (ModelState.IsValid)
            {
                service.Add(accessionRecord);
                return RedirectToAction("Index");
            }
            
            return View(accessionRecord);
        }

        // GET: AccessionRecords/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccessionRecord accessionRecord = service.GetAccessionRecordById(id);
            if (accessionRecord == null)
            {
                return HttpNotFound();
            }
            return View(accessionRecord);
        }

        // POST: AccessionRecords/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AccessionRecord accessionRecord)
        {
            if (ModelState.IsValid)
            {
                service.Update(accessionRecord);
                return RedirectToAction("Index");
            }
            return View(accessionRecord);
        }

        // GET: AccessionRecords/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccessionRecord accessionRecord = service.GetAccessionRecordById(id);
            if (accessionRecord == null)
            {
                return HttpNotFound();
            }
            return View(accessionRecord);
        }

        // POST: AccessionRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            service.Delete(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                service.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
