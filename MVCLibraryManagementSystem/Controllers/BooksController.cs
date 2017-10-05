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
using MVCLibraryManagementSystem.ViewModels;
using AutoMapper;
using PagedList;

namespace MVCLibraryManagementSystem.Controllers
{
    public class BooksController : Controller
    {
        private IBookService service;

        public BooksController()
        {
            this.service = new BookService(new LibraryContext());
        }

        public BooksController(IBookService _serv)
        {
            this.service = _serv;
        }

        // GET: Books
        public ActionResult Index(int? page=1)
        {
            var books = service.GetAllBooks();

            if (page == null)
                page = 1;

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            books = books.OrderBy(b => b.BookId).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return View(books);
        }

        // GET: Books/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = service.GetBookById(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: Books/Create
        public ActionResult Create(int? page)
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // CREATE IS HERE YOU STUPID MORON!
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude="BookId")] Book book)
        {
            if (ModelState.IsValid)
            {
                service.Add(book);
                return RedirectToAction("Details", new { id = book.BookId });
            }

            return View(book);
        }

        // GET: Books/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = service.GetBookById(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Book book)
        {
            if (ModelState.IsValid)
            {
                service.Update(book);
                return RedirectToAction("Index");
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = service.GetBookById(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Books/Delete/5
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
