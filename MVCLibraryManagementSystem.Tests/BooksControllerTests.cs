using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVCLibraryManagementSystem.Models;
using MVCLibraryManagementSystem.DAL;
using MVCLibraryManagementSystem.Controllers;
using Moq;

namespace MVCLibraryManagementSystem.Tests
{
    [TestClass]
    public class BooksControllerTests
    {
        public List<Book> books;
        [TestInitialize]
        public void Init()
        {
            books = new List<Book>()
            {
                new Book() { Item = new Item() { Title = "testBook1 "}, BookId = 1},
                new Book() { Item = new Item() { Title = "testBook2 "}, BookId = 2},
                new Book() { Item = new Item() { Title = "testBook3 "}, BookId = 3},
                new Book() { Item = new Item() { Title = "testBook4 "}, BookId = 4},
                new Book() { Item = new Item() { Title = "testBook5 "}, BookId = 5},
            };

        }
        // This is why Repos are needed, so that we don't access this directly
        // and we can mock data whenever we need to.
        private LibraryContext context = new LibraryContext();

        /// <summary>
        /// Makes sure that the Create controller redirects to the details of the new book (that has id 6) upon success.
        /// </summary>
        [TestMethod]
        public void TestCreateReturnsDetails()
        {
            var mock = new Mock<DAL.IBookService>();

            Book newBook = new Book() { Item = new Item() { Title = "testBook6 " }, BookId = 6 };

            BooksController controller = new BooksController(mock.Object);

            var result = controller.Create(newBook) as RedirectToRouteResult;
            Assert.AreEqual("Details", result.RouteValues["action"]);
            Assert.AreEqual(6, result.RouteValues["id"]);
        }

        /// <summary>
        /// By default the Book Index view doesn't list items. This makes sure it does
        /// </summary>
        [TestMethod]
        public void TestBookIndexHasItems()
        {
            var mock = new Mock<DAL.IBookService>();
            

            mock.Setup(m => m.GetAllBooks()).Returns(books);
            BooksController controller = new BooksController(mock.Object);

            var viewResult = controller.Index() as ViewResult;

            List<Book> booksReturned = (List<Book>)viewResult.Model;

            CollectionAssert.AreEquivalent(booksReturned, books);
                
        }

        /// <summary>
        /// This code will not compile, however it needs to because Index neads a search searchString
        /// Makes sure that only one book is returned because there is only one book where the Title contains
        /// a "5"
        /// </summary>
        [TestMethod]
        public void TestBookIndexHasSearch()
        {
            var mock = new Mock<IBookService>();

            mock.Setup(m => m.GetAllBooks()).Returns(books);

            dynamic controller = new BooksController(mock.Object);
            var viewResult = controller.Index(searchString: "5") as ViewResult;
            List<Book> booksReturned = (List<Book>)viewResult.Model;
            Assert.AreEqual(1, booksReturned.Count);
        }

        /// <summary>
        /// This code will not compile, however it needs to because Index neads a search searchString
        /// </summary>
        [TestMethod]
        public void TestBookIndexHasPagination()
        {
            var mock = new Mock<IBookService>();
            
            mock.Setup(m => m.GetAllBooks()).Returns(books);

            dynamic controller = new BooksController(mock.Object);
            var viewResult = controller.Index(3) as ViewResult;
            List<Book> booksReturned = (List<Book>)viewResult.Model;

            Assert.AreEqual(10, booksReturned.Count);

        }

        /// <summary>
        /// Test that edit redirects to the details of the edited book.
        /// </summary>
        [TestMethod]
        public void TestEditBooks()
        {
            var mock = new Mock<IBookService>();

            int id = 3;

            mock.Setup(m => m.GetBookById((int)It.Is<Int32>(i => i == id))).Returns(books.Find(b => b.BookId == id));
           
            BooksController controller = new BooksController(mock.Object);

            var result = controller.Edit(mock.Object.GetBookById(id)) as RedirectToRouteResult;
            Assert.AreEqual("Details", result.RouteValues["action"]);
            Assert.AreEqual(id, result.RouteValues["id"]);            
        }
    }
}

