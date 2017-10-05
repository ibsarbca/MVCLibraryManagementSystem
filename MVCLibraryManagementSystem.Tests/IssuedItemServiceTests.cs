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

        //TODO:
        // Test that Service has GetLateFee methods


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
        }

        [TestMethod]
        public void TestGetUnIssuedAccRecords()
        {
            // This adds an accession record for a copy that is issued, so it should not be in the
            // list of records that is issued.
            AccessionRecord issuedRecord = new AccessionRecord() { Item = item, AccessionRecordId = 18 };            
            AccessionRecord neverIssued = new AccessionRecord() { Item = item, AccessionRecordId = 19 };
            accessionRecords.Add(issuedRecord);
            accessionRecords.Add(neverIssued);

            // For this test, we set all issuedItems as "Returned" except 1
            foreach(var i in issuedItems)
            {
                i.IsReturned = true;
            }

            // And add a new one, where "Returned" = false, i.e. it a member has borrowed it.
            issuedItems.Add(new IssuedItem() { AccessionRecord = issuedRecord, IsReturned = false });

            // The following, seemingly complicated setup set's up a Mock LibraryContext object. To make sure this is possible, all LibraryContext members were changed to
            // virtual. mockSet and mockARSet are fake instances of DbSet<IssuedItem> and DbSet<AccessionRecord> which are required by LibraryContext, and the methods in IssuedItemService
            // So here we set up mockSet and mockARSet, pass it to the Mock Library Context and pass the fake LibraryContext, to IssuedItemService. I know this is a lot of code, but it is
            // proof that the GetUnIssuedAccRecords() method works which is important because it has some complicated logic.
            var mockSet = new Mock<DbSet<IssuedItem>>();
            var mockARSet = new Mock<DbSet<AccessionRecord>>();

            mockSet.As<IQueryable<IssuedItem>>().Setup(m => m.Expression).Returns(issuedItems.AsQueryable().Expression);
            mockSet.As<IQueryable<IssuedItem>>().Setup(m => m.ElementType).Returns(issuedItems.AsQueryable().ElementType);
            mockSet.As<IQueryable<IssuedItem>>().Setup(m => m.GetEnumerator()).Returns(issuedItems.AsQueryable().GetEnumerator());

            mockARSet.As<IQueryable<AccessionRecord>>().Setup(m => m.Expression).Returns(accessionRecords.AsQueryable().Expression);
            mockARSet.As<IQueryable<AccessionRecord>>().Setup(m => m.ElementType).Returns(accessionRecords.AsQueryable().ElementType);
            mockARSet.As<IQueryable<AccessionRecord>>().Setup(m => m.GetEnumerator()).Returns(accessionRecords.AsQueryable().GetEnumerator());


            var mockContext = new Mock<LibraryContext>();
            mockContext.SetupGet(m => m.IssuedItems).Returns(mockSet.Object);
            mockContext.SetupGet(m => m.AccessionRecords).Returns(mockARSet.Object);

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

        [TestMethod]
        public void TestGetDueDate()
        {
            Member studentMember = new Member() { MemberType = MEMBERTYPE.STUDENT };
            Member facultyMember = new Member() { MemberType = MEMBERTYPE.FACULTY };
            IssuedItem studentItem = new IssuedItem() { AccessionRecord = accessionRecords[0], IssuedItemId = 21, Member = studentMember };
            IssuedItem facultyItem = new IssuedItem() { AccessionRecord = accessionRecords[0], IssuedItemId = 22, Member = facultyMember };

            dynamic service = new IssuedItemService(accRecMock.Object);
            DateTime returnDateStudent = service.GetDueDate(studentItem).ToString("dd/MM/yyyy");
            DateTime returnDateFaculty = service.GetDueDate(facultyItem).ToString("dd/MM/yyyy");

            // Student due date should be 7 days from today.
            Assert.IsTrue(returnDateStudent.Subtract(studentItem.IssueDate).Days == 7);

            // Faculty due date should be 90 days from now
            Assert.IsTrue(returnDateFaculty.Subtract(facultyItem.IssueDate).Days == 90);
        }

        [TestMethod]
        public void TestGetLateFee()
        {
            int lateFee = 5;
            
            IssuedItem someItem = new IssuedItem() { AccessionRecord = accessionRecords[0], IssuedItemId = 21, Member = member, LateFeePerDay = lateFee};
            
            dynamic service = new IssuedItemService(accRecMock.Object);
            DateTime returnDateStudent = service.GetDueDate(someItem).ToString("dd/MM/yyyy");

            DateTime tenDaysLate = returnDateStudent.Add(TimeSpan.FromDays(10));

            Assert.IsTrue(service.GetLateFee(someItem) == lateFee * 10);
        }

    }
}
