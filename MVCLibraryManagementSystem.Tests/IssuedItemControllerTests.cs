using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVCLibraryManagementSystem.Models;
using MVCLibraryManagementSystem.Controllers;
using MVCLibraryManagementSystem.DAL;
using Moq;

namespace MVCLibraryManagementSystem.Tests
{
    [TestClass]
    public class IssuedItemControllerTests
    {
        Mock<IIssuedItemService> mock = new Mock<IIssuedItemService
>();
        List<IssuedItem> issuedItems = new List<IssuedItem>();

        Item item = new Item() { Title = "Item To Issue", ItemId = 1 };
        List<AccessionRecord> accessionRecords = new List<AccessionRecord>();
        Member member = new Member() { Name = "Test Member", MemberId = 100, MemberType = MEMBERTYPE.FACULTY };

        //TODO:
        // Test Default Values
        // Test that Service has GetDueDate and GetLateFee methods

        [TestInitialize]
        public void Init()
        {
            accessionRecords = new List<AccessionRecord>()
            {
                new AccessionRecord() {Item = item, AccessionRecordId = 10},
                new AccessionRecord() {Item = item, AccessionRecordId = 11},
                new AccessionRecord() {Item = item, AccessionRecordId = 12},
                new AccessionRecord() {Item = item, AccessionRecordId = 13}
            };

            issuedItems = new List<IssuedItem>()
            {
                new IssuedItem() { AccessionRecord = accessionRecords[0], LateFeePerDay = 5, Member = member, IssuedItemId = 20, IsReturned = true },
                new IssuedItem() { AccessionRecord = accessionRecords[1], LateFeePerDay = 5, Member = member, IssuedItemId = 21, IsReturned = true },
                new IssuedItem() { AccessionRecord = accessionRecords[2], LateFeePerDay = 5, Member = member, IssuedItemId = 22, IsReturned = true},
                new IssuedItem() { AccessionRecord = accessionRecords[3], LateFeePerDay = 5, Member = member, IssuedItemId = 23, IsReturned = true },
            };
        }

        /// <summary>
        /// Test that the Index page has items and those items have all the required properties
        /// </summary>
        [TestMethod]
        public void TestIndexHasItems()
        {
            IssuedItemsController controller = new IssuedItemsController(mock.Object);

            var viewResult = controller.Index() as ViewResult;

            IEnumerable<IssuedItem> recordsReturned = (IEnumerable<IssuedItem>)
viewResult.Model;

            // Note: I suppose this should only check against required properties,
            // but maybe checking all properties is a better idea, I don't know.
            foreach(var issuedItem in recordsReturned)
            {
                Assert.IsNotNull(issuedItem.IssueDate);
                Assert.IsNotNull(issuedItem.Member);
                Assert.IsNotNull(issuedItem.AccessionRecord);
            }
        }

        /// <summary>
        /// Test that the Create method requires an Item ID. The behaviour of the Create action (both GET and POST) should be that it takes an Item ID 
        /// and creates a selects Accession Record that is NOT already issued. To do this, the IIssuedItemService class should implement a method that
        /// returns an random accession record that isn't in the already issued list. The Test for this, should be in will be in it's own TestClass
        /// </summary>
        [TestMethod]
        public void TestCreateTakesItemId()
        {// this should take an item id, not ar id.
            dynamic controller = new IssuedItemsController(mock.Object);

            controller.Create(itemid: 10);
        }

        /// <summary>
        /// Tests that Create calls Add on the IIssuedItemRepository
        /// </summary>
        [TestMethod]
        public void TestCreateWorks()
        {
            var controller = new IssuedItemsController(mock.Object);
            IssuedItem toAdd = new IssuedItem { AccessionRecord = accessionRecords[0], Member = member, IssuedItemId = 15, IssueDate = DateTime.Now.Date };

            controller.Create(toAdd);
            mock.Verify(m => m.Add(It.IsAny<IssuedItem>()), Times.Once);
        }

        [TestMethod]
        public void TestCreateHasProperModel()
        {
            dynamic controller = new IssuedItemsController(mock.Object);
            var result = controller.Create(10) as ViewResult;
            var newRecord = result.Model as IssuedItem;

            Assert.IsNotNull(newRecord.IssueDate);
            Assert.IsNotNull(newRecord.AccessionRecord);
            Assert.IsNotNull(newRecord.Member);
            Assert.IsFalse(newRecord.IsReturned);
        }

        /// <summary>
        /// The code in the try method tests that the IssuedItems controller has an Index method, that takes a search string, and it returns a subset of all records
        /// in the model, which here should be an IEnumerable of IssuedItems.
        /// A dynamic is used because the method might not exist, in which case an exception is thrown.
        /// This is to make sure that the code compiles, even if the method does not exist.
        /// </summary>
        [TestMethod]
        public void TestIndexHasSearch()
        {
            dynamic controller = new IssuedItemsController(mock.Object);
            var viewResult = controller.index(searchString: "Test") as ViewResult;
            List<IssuedItem> recordsReturned = (List<IssuedItem>)viewResult.Model;
            List<IssuedItem> allRecords = (List<IssuedItem>)mock.Object.GetAllIssuedItems();

            CollectionAssert.IsSubsetOf(recordsReturned, allRecords);
        }

        /// <summary>
        /// Test that the Index returns a collection of ten elements for page "3" for example. That is to say, each page should have 10 elements.
        /// </summary>
        [TestMethod]
        public void TestIndexHasPagination()
        {
            dynamic controller = new IssuedItemsController(mock.Object);
            var viewResult = controller.index(page: 3) as ViewResult;
            List<IssuedItem> recordsReturned = (List<IssuedItem>)viewResult.Model;
            List<IssuedItem> allRecords = (List<IssuedItem>)mock.Object.GetAllIssuedItems();

            Assert.IsTrue(recordsReturned.Count <= 10);
        }

        /// <summary>
        /// Edit must have the required properties, which cannot be changed.
        /// </summary>
        [TestMethod]
        public void TestEditHasProperModel()
        {
            dynamic controller = new IssuedItemsController(mock.Object);
            var viewResult = controller.Edit(id: 1) as ViewResult;

            var model = viewResult.Model as IssuedItem;
            Assert.IsNotNull(model.AccessionRecord);
            Assert.IsNotNull(model.Member);
            Assert.IsNotNull(model.IssueDate);
        }

        
        [TestMethod]
        public void TestEditCallsUpdate()
        {
            dynamic controller = new IssuedItemsController(mock.Object);
            mock.Setup(m => m.Update(It.IsAny<IssuedItem>()));

            controller.Edit(accessionRecords[0]);
            mock.Verify(m => m.Update(It.IsAny<IssuedItem>()), Times.Once);
        }

        /// <summary>
        /// Test that the SetReturned method of the controller updates the model. This test needs to be worked on.
        /// </summary>
        [TestMethod]
        public void TestReturnedIsSet()
        {
            dynamic controller = new IssuedItemsController(mock.Object);
            mock.Setup(m => m.Update(It.IsAny<IssuedItem>()));

            IssuedItem returnedItem = new IssuedItem() { AccessionRecord = accessionRecords[0], IssuedItemId = 1, LateFeePerDay = 5, IsReturned = false };

            var result = controller.SetReturned(id: 1) as JsonResult;
            
            mock.Verify(m => m.Update(It.IsAny<IssuedItem>()), Times.Once);
            // Assert.IsTrue(result.Data) // or something;
        }
    }
}
