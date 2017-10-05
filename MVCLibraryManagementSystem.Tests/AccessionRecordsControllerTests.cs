using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Diagnostics;
using System.Reflection;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVCLibraryManagementSystem.Models;
using MVCLibraryManagementSystem.DAL;
using MVCLibraryManagementSystem.Controllers;
using Moq;
using System.Web.Routing;

namespace MVCLibraryManagementSystem.Tests
{
    [TestClass]
    public class AccessionRecordsControllerTests
    {

        // This is why Repos are needed, so that we don't access this directly
        // and we can mock data whenever we need to.
        private LibraryContext context = new LibraryContext();
        Mock<IAccessionRecordService> mock = new Mock<IAccessionRecordService>();
        List<AccessionRecord> accessionRecords;
        [TestInitialize]
        public void Init()
        {
            Item item = new Item() {Title = "TestItem", ItemId = 1 };
            accessionRecords = new List<AccessionRecord>()
            {
                new AccessionRecord() {Item = item, AccessionRecordId = 10},
                new AccessionRecord() {Item = item, AccessionRecordId = 11},
                new AccessionRecord() {Item = item, AccessionRecordId = 12},
                new AccessionRecord() {Item = item, AccessionRecordId = 13}
            };

            mock.Setup(m => m.GetItemService().GetItemById(It.IsAny<Int32>())).Returns(new Item { Title = "Test", ItemId = 1 });
            mock.Setup(m => m.GetAllAccessionRecords()).Returns(accessionRecords);
        }

        /// <summary>
        /// By default the AccessionRecords Index view doesn't list items. This makes sure it does
        /// </summary>
        [TestMethod]
        public void TestIndexHasItems()
        {
            AccessionRecordsController controller = new AccessionRecordsController(mock.Object);

            var viewResult = controller.Index() as ViewResult;

            IEnumerable<AccessionRecord> recordsReturned = (IEnumerable<AccessionRecord>)viewResult.Model;

            foreach (var AccessionRecord in recordsReturned)
            {
                Assert.IsNotNull(AccessionRecord.Item.Title);
            }
        }

        /// <summary>
        /// An Accession Record cannot be created for an Item that doesn't exist. Therefore a new Accession Record must
        /// already have an Item associated with it. This makes sure that the Create GET action method takes an ItemId.
        /// This test will not compile otherwise.
        /// </summary>
        [TestMethod]
        public void TestCreateTakesItemId()
        {
            var mock = new Mock<IAccessionRecordService>();
            mock.Setup(m => m.GetItemService().GetItemById(It.IsAny<Int32>())).Returns(new Item{ Title = "Test", ItemId = 1});

            AccessionRecordsController controller = new AccessionRecordsController(mock.Object);
            var result = controller.Create(itemid: 1);
        }

        /// <summary>
        /// Makes sure that create calls Add on the IAccessionRecordRepository
        /// </summary>
        [TestMethod]
        public void TestCreateWorks()
        {
            AccessionRecordsController controller = new AccessionRecordsController(mock.Object);
            AccessionRecord arToAdd = new AccessionRecord() { Item = new Item() {Title="Ignore this"}, AccessionRecordId = 1};
            mock.Setup(m => m.Add(It.IsAny<AccessionRecord>()));

            controller.Create(arToAdd);
            mock.Verify(m => m.Add(It.IsAny<AccessionRecord>()), Times.Once);

        }

        /// <summary>
        /// Make sure that when a Create View is called with ItemId, that item id is added to the model that will be created.
        /// See documentation for TestCreateTakesItemId() for more details.
        /// </summary>
        [TestMethod]
        public void TestCreateHasProperModel()
        {
            AccessionRecordsController controller = new AccessionRecordsController(mock.Object);
            var result = controller.Create(1) as ViewResult;
            var newRecord = result.Model as AccessionRecord;

            Assert.IsNotNull(newRecord.Item);
            Assert.IsNotNull(newRecord.DateOfReceipt);
        }

        /// <summary>
        /// The code in the try method tests that the Acc. Records controller has an Index method, that takes a search string, and it returns a subset of all records
        /// in the model, which here should be an IEnumerable of Accession Records.
        /// A dynamic is used because the method might not exist, in which case an exception is thrown.
        /// This is to make sure that the code compiles, even if the method does not exist.
        /// </summary>
        [TestMethod]
        public void TestAccessionRecordIndexHasSearch()
        {
            dynamic controller = new AccessionRecordsController(mock.Object);
            var viewResult = controller.Index(searchString: "Test") as ViewResult;
            List<AccessionRecord> recordsReturned = (List<AccessionRecord>)viewResult.Model;
            List<AccessionRecord> allRecords = (List<AccessionRecord>)this.mock.Object.GetAllAccessionRecords();

            CollectionAssert.IsSubsetOf(recordsReturned, allRecords);
            
        }

        /// <summary>
        /// This tests checks if the Index has a page parameter, that takes an int argument (such as 3) and returns 10 records. Those 10 records would be the records
        /// for page "3", for example. Basically it checks pagination and that each page has 10 records.
        /// </summary>
        [TestMethod]
        public void TestAccessionRecordIndexHasPagination()
        {
            dynamic controller = new AccessionRecordsController(mock.Object);
            var viewResult = controller.Index(page: 3) as ViewResult;
            List<AccessionRecord> recordsReturned = (List<AccessionRecord>)viewResult.Model;
            List<AccessionRecord> allRecords = (List<AccessionRecord>)mock.Object.GetAllAccessionRecords();

            Assert.IsTrue(recordsReturned.Count <= 10);
        }

        /// <summary>
        /// Make sure that when editing, the view has an Item already set.
        /// Note: The Default Edit method generated by scaffolding should have the "Source", "DateOfReciept" and "Price" option so they are not tested here.
        /// </summary>
        [TestMethod]
        public void TestEditHasItem()
        {
            dynamic controller = new AccessionRecordsController(mock.Object);
            var viewResult = controller.Edit(id: 1) as ViewResult;

            var model = viewResult.Model as AccessionRecord;
            Assert.IsNotNull(model.Item);
        }

        /// <summary>
        /// Makes sure Edit(Acc Record) calls Update()
        /// </summary>
        [TestMethod]
        public void TestEditCallsUpdate()
        {
            AccessionRecordsController controller = new AccessionRecordsController(mock.Object);
            AccessionRecord arToAdd = new AccessionRecord() { Item = new Item() { Title = "Ignore this" }, AccessionRecordId = 1 };
            mock.Setup(m => m.Update(It.IsAny<AccessionRecord>()));

            controller.Edit(arToAdd);
            mock.Verify(m => m.Update(It.IsAny<AccessionRecord>()), Times.Once);
        }
    }
}
