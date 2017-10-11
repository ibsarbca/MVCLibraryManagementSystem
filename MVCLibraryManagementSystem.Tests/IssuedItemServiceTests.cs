using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Diagnostics;
using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVCLibraryManagementSystem.Models;
using MVCLibraryManagementSystem.Controllers;
using MVCLibraryManagementSystem.DAL;
using Moq;

namespace MVCLibraryManagementSystem.Tests
{
    [TestClass]
    public class IssuedItemServiceTests
    {
        // We're testing IssuedItemService, so we need a fake IAccessionRecordService
        Mock<IAccessionRecordService> accRecMock = new Mock<IAccessionRecordService>(MockBehavior.Loose);
        List<IssuedItem> issuedItems = new List<IssuedItem>();

        Item item = new Item() { Title = "Item To Issue", ItemId = 1 };
        List<AccessionRecord> accessionRecords = new List<AccessionRecord>();
        Member member = new Member() { Name = "Test Member", MemberId = 100, MemberType = MEMBERTYPE.FACULTY };

        // Mock DBSets and LibraryContext to pass to service
        Mock<DbSet<IssuedItem>> mockSet = new Mock<DbSet<IssuedItem>>();
        Mock<DbSet<AccessionRecord>> mockARSet = new Mock<DbSet<AccessionRecord>>();
        Mock<LibraryContext> mockContext = new Mock<LibraryContext>();

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
                new IssuedItem() { AccessionRecord = accessionRecords[0], LateFeePerDay = 5, Member = member, IssuedItemId = 20 },
                new IssuedItem() { AccessionRecord = accessionRecords[1], LateFeePerDay = 5, Member = member, IssuedItemId = 21 },
                new IssuedItem() { AccessionRecord = accessionRecords[2], LateFeePerDay = 5, Member = member, IssuedItemId = 22 },
                new IssuedItem() { AccessionRecord = accessionRecords[3], LateFeePerDay = 5, Member = member, IssuedItemId = 23 },
            };


            // Set up Item Mock
            accRecMock.Setup(m => m.GetAllAccessionRecords()).Returns(accessionRecords);

            // The following, seemingly complicated setup set's up a Mock LibraryContext object. 
            // To make sure this is possible, all LibraryContext members were changed to
            // virtual. mockSet and mockARSet are fake instances of DbSet<IssuedItem> and DbSet<AccessionRecord> which are required
            // by LibraryContext, and the methods in IssuedItemService
            // So here we set up mockSet and mockARSet, pass it to the Mock Library Context and pass the fake LibraryContext, to
            // IssuedItemService. I know this is a lot of code, but it is
            // proof that the GetUnIssuedAccRecords() method works which is important because it has some complicated logic.
            
            mockSet.As<IQueryable<IssuedItem>>().Setup(m => m.Expression).Returns(issuedItems.AsQueryable().Expression);
            mockSet.As<IQueryable<IssuedItem>>().Setup(m => m.ElementType).Returns(issuedItems.AsQueryable().ElementType);
            mockSet.As<IQueryable<IssuedItem>>().Setup(m => m.GetEnumerator()).Returns(issuedItems.AsQueryable().GetEnumerator());

            mockARSet.As<IQueryable<AccessionRecord>>().Setup(m => m.Expression).Returns(accessionRecords.AsQueryable().Expression);
            mockARSet.As<IQueryable<AccessionRecord>>().Setup(m => m.ElementType).Returns(accessionRecords.AsQueryable().ElementType);
            mockARSet.As<IQueryable<AccessionRecord>>().Setup(m => m.GetEnumerator()).Returns(accessionRecords.AsQueryable().GetEnumerator());

            mockContext.SetupGet(m => m.IssuedItems).Returns(mockSet.Object);
            mockContext.SetupGet(m => m.AccessionRecords).Returns(mockARSet.Object);
        }

        [TestMethod]
        public void TestGetUnIssuedAccRecords()
        {
            // For this test, we set all issuedItems as "Returned"
            foreach (var i in issuedItems)
            {
                i.IsReturned = true;
            }

            // This adds an accession record for a copy that is issued
            AccessionRecord issuedRecord = new AccessionRecord() { Item = item, AccessionRecordId = 18 };
            accessionRecords.Add(issuedRecord);

            // And add a new one, where "Returned" = false, i.e. it a member has borrowed it.
            // i.e, a member borrows the new copy, hence isReturned = false
            AccessionRecord neverIssued = new AccessionRecord() { Item = item, AccessionRecordId = 19 };
            accessionRecords.Add(neverIssued);
            // Now we add an issuedItem which is borrowed. We're simulating a situation where all Items are returned,
            // and a member borrows one Item (whose Acc. Record is issuedRecord).
            issuedItems.Add(new IssuedItem() { AccessionRecord = issuedRecord, IsReturned = false });

            var service = new IssuedItemService(mockContext.Object);

            IEnumerable<int> recordIds = service.GetAllIssuableAccRecords().Select(r => r.AccessionRecordId);
            foreach(var rec in recordIds)
            {
                Debug.WriteLine(rec);
            }

            // Assert that the record that was never issued is also included
            Assert.IsTrue(recordIds.Contains(neverIssued.AccessionRecordId));
            // Assert that the returned Acession Record is NOT included
            Assert.IsFalse(recordIds.Contains(issuedRecord.AccessionRecordId));
        }

        /// <summary>
        /// Tests that the GetRandomIssuableAccRecord, gets a random accession record which is issuable and
        /// whose ItemId is 1.
        /// </summary>
        [TestMethod]
        public void TestGetRandomIssuableAccRecord()
        {
            dynamic service = new IssuedItemService(mockContext.Object);
            // For this test, we set all issuedItems as "Returned"
            foreach (var i in issuedItems)
            {
                i.IsReturned = true;
            }
            AccessionRecord ar = service.GetRandomIssuableAccRecord(itemid: 1);
            Assert.AreEqual(1, ar.Item.ItemId);
        }

        [TestMethod]
        public void TestGetDueDate()
        {
            Member studentMember = new Member() { MemberType = MEMBERTYPE.STUDENT };
            Member facultyMember = new Member() { MemberType = MEMBERTYPE.FACULTY };
            IssuedItem studentItem = new IssuedItem() { AccessionRecord = accessionRecords[0], IssuedItemId = 21, Member = studentMember };
            IssuedItem facultyItem = new IssuedItem() { AccessionRecord = accessionRecords[0], IssuedItemId = 22, Member = facultyMember };

            dynamic service = new IssuedItemService(accRecMock.Object);
            DateTime returnDateStudent = service.GetDueDate(studentItem);
            DateTime returnDateFaculty = service.GetDueDate(facultyItem);

            // Student due date should be 7 days from today.
            Assert.IsTrue(returnDateStudent.Subtract(studentItem.IssueDate).Days == 7);

            // Faculty due date should be 90 days from now
            Assert.IsTrue(returnDateFaculty.Subtract(facultyItem.IssueDate).Days == 90);
        }

        [TestMethod]
        public void TestGetLateFee()
        {
            int lateFee = 5;
            Member studentMember = new Member() { MemberType = MEMBERTYPE.STUDENT };

            DateTime tenDaysAgo = DateTime.Now.Subtract(TimeSpan.FromDays(10));
            IssuedItem someItem = new IssuedItem() {
                AccessionRecord = accessionRecords[0],
                IssuedItemId = 21,
                Member = studentMember,
                LateFeePerDay = lateFee,
                IssueDate = tenDaysAgo
                // Setting the issueDate to 10 days ago makes the number of late days to be
                // 3 for a student member.
            };

            
            dynamic service = new IssuedItemService(accRecMock.Object);

            // Number of days from due date that have passed i.e. how may days ago
            // was the due date
            int lateDays = DateTime.Now.Subtract(service.GetDueDate(someItem)).Days;

            Assert.AreEqual(lateFee * lateDays, service.GetLateFee(someItem));
        }

    }
}
