using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVCLibraryManagementSystem.DAL;
using MVCLibraryManagementSystem.Models;
using MVCLibraryManagementSystem.Controllers;

namespace MVCLibraryManagementSystem.Tests
{
    [TestClass]
    public class ModelTests
    {
        /// <summary>
        /// Run the validation on a model and return the results
        /// </summary>
        /// <param name="model">Model to Validate</param>
        /// <returns>List<> containing objects of type ValidationResult</returns>
        public List<ValidationResult> GetValidationResults(object model)
        {
            var results = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, results);

            return results;
        }

        /// <summary>
        /// Make sure an Item without a Title does not validate
        /// </summary>
        [TestMethod]
        public void TestItemModelValidation()
        {
            Item item = new Item()
            {
                Year = 000
            };

            var results = GetValidationResults(item);
            // There should only be one and it should be title

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Title", results.First().MemberNames.First());
        }

        /// <summary>
        /// Make sure that a book with only a title works. This is the bare minimum.
        /// </summary>
        [TestMethod]
        public void TestBookModelValidation()
        {
            Book book = new Book()
            {
                Item = new Item() { Title = "TestBook" }
            };

            var results = GetValidationResults(book);

            Assert.AreEqual(0, results.Count);
        }

        /// <summary>
        /// Test Member without a Name and MemberType does not work
        /// </summary>
        [TestMethod]
        public void TestMemberModelValidation()
        {
            Member member = new Member()
            {
                MemberId = 0
            };

            var results = GetValidationResults(member);
            Debug.WriteLine(results);
            Assert.AreEqual(2, results.Count);
        }

        /// <summary>
        /// Test Newspaper needs an Item and a date
        /// </summary>
        [TestMethod]
        public void TestNewspaperModelValidation()
        {
            Newspaper paper = new Newspaper()
            {
                Category = CATEGORIES.ENTERTAINMENT
            };

            var results = GetValidationResults(paper);
            Assert.AreEqual(2, results.Count);
        }

        /// <summary>
        /// Test QuestionPaper needs a subject and a month
        /// </summary>
        [TestMethod]
        public void TestQuestionPaperModelValidation()
        {
            QuestionPaper paper = new QuestionPaper()
            {
                Department = DEPARTMENTS.BBA,
                Item = new Item() { Title = "Question Paper" }
            };

            var results = GetValidationResults(paper);
            Assert.IsTrue(results.Any(v => v.MemberNames.Contains("Month")));
            Assert.IsTrue(results.Any(v => v.MemberNames.Contains("Subject")));
        }

        /// <summary>
        /// Test IssuedItem has an AccessionId, Member and IssuedDate
        /// </summary>
        [TestMethod]
        public void TestIssuedItemModelValidation()
        {
            Item item = new Item { Title = "Item To issue", ItemId = 1};
            IssuedItem issuedItem = new IssuedItem()
            {
                LateFeePerDay = 0
            };

            var results = GetValidationResults(issuedItem);

            // The results.Any part returns true of any object in the results
            // list has a MemberNames property which contains the word "Member"
            // and the others
            Assert.IsTrue(results.Any(v => v.MemberNames.Contains("Member")));
            Assert.IsTrue(results.Any(v => v.MemberNames.Contains("AccessionRecord")));
            Assert.IsTrue(results.Any(v => v.MemberNames.Contains("IssueDate")));
        }

        [TestMethod]
        public void TestIssuedItemDefaults()
        {
            DateTime currentDate = DateTime.Now.Date;

            IssuedItem issuedItem = new IssuedItem();
            Assert.AreEqual(currentDate.ToString("dd/MM/yyyy"), issuedItem.IssueDate.ToString("dd/MM/yyyy"));

            Assert.IsFalse(issuedItem.IsReturned);
            Assert.IsNull(issuedItem.IsLateFeePaid);
        }
    }
} 
