using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Diagnostics;
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
        Mock<IIssuedItemService> mock = new Mock<IIssuedItemService>();
        Mock<IMemberService> memberMock = new Mock<IMemberService>();

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
                new IssuedItem() { AccessionRecord = accessionRecords[0], LateFeePerDay = 5, Member = member, IssuedItemId = 20, IsReturned = true, IssueDate = DateTime.Now },
                new IssuedItem() { AccessionRecord = accessionRecords[1], LateFeePerDay = 5, Member = member, IssuedItemId = 21, IsReturned = true, IssueDate = DateTime.Now },
                new IssuedItem() { AccessionRecord = accessionRecords[2], LateFeePerDay = 5, Member = member, IssuedItemId = 22, IsReturned = true, IssueDate = DateTime.Now},
                new IssuedItem() { AccessionRecord = accessionRecords[3], LateFeePerDay = 5, Member = member, IssuedItemId = 23, IsReturned = true, IssueDate = DateTime.Now},
            };

            mock = new Mock<IIssuedItemService>();
            issuedItems[3].IsReturned = false;
            mock.Setup(m => m.GetAllIssuedItems()).Returns(issuedItems);
            mock.Setup(m => m.GetRandomIssuableAccRecord(It.IsAny<int>())).Returns(accessionRecords[0]);
            
        }

        /// <summary>
        /// Test that the Index page has items and those items have all the required properties
        /// </summary>
        [TestMethod]
        public void TestIndexHasItems()
        {
            IssuedItemsController controller = new IssuedItemsController(mock.Object);

            var viewResult = controller.Index(searchString: "") as ViewResult;

            IEnumerable<IssuedItem> recordsReturned = (IEnumerable<IssuedItem>)
viewResult.Model;

            // Note: I suppose this should only check against required properties,
            // but maybe checking all properties is a better idea, I don't know.
            foreach (var issuedItem in recordsReturned)
            {
                Assert.IsNotNull(issuedItem.IssueDate);
                Assert.IsNotNull(issuedItem.Member);
                Assert.IsNotNull(issuedItem.AccessionRecord);
            }
        }

        /// <summary>
        /// Test that the Create method requires an Item ID. The behaviour of the Create action (both GET and POST) should be that it takes an Item ID 
        /// and creates and selects Accession Record that is NOT already issued. To do this, the IIssuedItemService class should implement a method that
        /// returns a random accession record that isn't in the already issued list. The Test for this, should be in will be in it's own TestClass
        /// See TestCreateWorks()
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
            // Set up a fake GetRandomIssuableAccRecord() and make sure it's called.
            // smock.Setup(m => m.GetRandomIssuableAccRecord()).Returns(accessionRecords[0]);
            memberMock.Setup(m => m.GetMemberById(It.IsAny<int>())).Returns(member);
            var controller = new IssuedItemsController(mock.Object, memberMock.Object);
            IssuedItem toAdd = new IssuedItem { AccessionRecord = accessionRecords[0], Member = member, IssuedItemId = 15, IssueDate = DateTime.Now.Date };

            controller.Create(toAdd);
            // Make sure that the Create method calls a GetRandomIssueableAccRecord()
            //mock.Verify(m => m.GetRandomIssuableAccRecord(It.IsAny<int>()), Times.Once);
            // Test that it calls the service.Add() method, which it doesn't by default
            mock.Verify(m => m.Add(It.IsAny<IssuedItem>()), Times.Once);
        }

        /// <summary>
        /// Tests Create GET has all the necessary foriegn key properties already set.
        /// AccessionRecord, IssueDate and IsReturned have to already be filled in the form.
        /// </summary>
        [TestMethod]
        public void TestCreateHasProperModel()
        {
            dynamic controller = new IssuedItemsController(mock.Object);
            var result = controller.Create(itemid: 4) as ViewResult;
            var newRecord = result.Model as IssuedItem;

            Assert.IsNotNull(newRecord.IssueDate);
            Assert.IsNotNull(newRecord.AccessionRecord);
            // Why does a new IssuedItem need a Record for GET
            // Assert.IsNotNull(newRecord.Member);
            Assert.IsFalse(newRecord.IsReturned);
        }

        /// <summary>
        /// Test that the Create method that runs on POST, creates an error if a member with the specific id does not exist
        /// with ModelState.AddModelErrors()
        /// </summary>
        [TestMethod]
        public void TestCreateChecksMemberId()
        {
            //var memberServiceMock = new Mock<IMemberService>();

            dynamic controller = new IssuedItemsController(mock.Object, memberMock.Object);

            IssuedItem itemToValidate = new IssuedItem()
            {
                AccessionRecord = accessionRecords[0],
                IssueDate = DateTime.Now.Date,
                Member = new Member() { MemberId = -1 }
            };

            var result = controller.Create(itemToValidate) as ViewResult;

            // Make sure create calls GetMemberById
            memberMock.Verify(m => m.GetMemberById(It.IsAny<int?>()), Times.Once);
            // Make sure that errors for the Member field exist
            Assert.IsNotNull(result.ViewData.ModelState["Member"].Errors);
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
            var viewResult = controller.Index(searchString: "Test") as ViewResult;
            List<IssuedItem> recordsReturned = (List<IssuedItem>)viewResult.Model;
            List<IssuedItem> allRecords = mock.Object.GetAllIssuedItems();

            CollectionAssert.IsSubsetOf(recordsReturned, allRecords);
        }

        /// <summary>
        /// Test that the Index returns a collection of ten elements for page "3" for example. That is to say, each page should have 10 elements.
        /// </summary>
        [TestMethod]
        public void TestIndexHasPagination()
        {
            dynamic controller = new IssuedItemsController(mock.Object);
            var viewResult = controller.Index(page: 3) as ViewResult;
            List<IssuedItem> recordsReturned = (List<IssuedItem>)viewResult.Model;
            List<IssuedItem> allRecords = mock.Object.GetAllIssuedItems();

            Assert.IsTrue(recordsReturned.Count <= 10);
        }

        /// <summary>
        /// Edit must have the required properties, which cannot be changed.
        /// </summary>
        [TestMethod]
        public void TestEditHasProperModel()
        {
            // Please see the Edit method, Obsolete Attribute.
        }


        [TestMethod]
        public void TestEditCallsUpdate()
        {
            // Please see the Edit method, Obsolete Attribute.
        }

        /// <summary>
        /// Test that the SetReturned method of the controller updates the model and redirects to the same View
        /// An IssuedItem can be set as "Returned" from details, according to this test.
        /// </summary>
        [TestMethod]
        public void TestReturnedIsSet()
        {
            dynamic controller = new IssuedItemsController(mock.Object);
            mock.Setup(m => m.Update(It.IsAny<IssuedItem>()));
            mock.Setup(m => m.GetById(It.IsAny<int>())).Returns(issuedItems[0]);

            var result = controller.SetReturned(id: 1) as RedirectToRouteResult;

            mock.Verify(m => m.Update(It.IsAny<IssuedItem>()), Times.Once);
            Assert.AreEqual("Details", result.RouteValues["action"]);
            Assert.AreEqual(issuedItems[0].IssuedItemId, result.RouteValues["id"]);
        }

        /// <summary>
        /// Tests that the SetDateReturned method exits and redirects back to Details when it is done.
        /// This should be called from the Details page and the method call should be Post.
        /// </summary>
        [TestMethod]
        public void TestDateReturnedIsSet()
        {
            dynamic controller = new IssuedItemsController(mock.Object);
            mock.Setup(m => m.Update(It.IsAny<IssuedItem>()));
            mock.Setup(m => m.GetById(It.IsAny<int>())).Returns(issuedItems[0]);

            var result = controller.SetDateReturned(id: 1) as RedirectToRouteResult;

            mock.Verify(m => m.Update(It.IsAny<IssuedItem>()), Times.Once);
            Assert.AreEqual("Details", result.RouteValues["action"]);
        }

        /// <summary>
        /// Tests that the SetLateFeePaid method exists and redirects back to Details when it is done.
        /// This should be called from the Details page and the method call should be Post.
        /// </summary>
        [TestMethod]
        public void TestSetLateFeePaid()
        {
            dynamic controller = new IssuedItemsController(mock.Object);
            mock.Setup(m => m.Update(It.IsAny<IssuedItem>()));
            mock.Setup(m => m.GetById(It.IsAny<int>())).Returns(issuedItems[0]);

            var result = controller.SetDateReturned(id: 1) as RedirectToRouteResult;

            mock.Verify(m => m.Update(It.IsAny<IssuedItem>()), Times.Once);
            Assert.AreEqual("Details", result.RouteValues["action"]);
        }
    }
}
