using System;
using System.Collections.Generic;
using System.Linq;
using InterviewTestPagination.Models;
using InterviewTestPagination.Models.Todo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InterviewTestPagination.Tests.Models {
    /// <summary>
    /// Unity testing for <see cref="InterviewTestPagination.Models.Todo.TodoService" />
    /// (the in-memory datasource is seeded with 55 todos).
    /// </summary>
    [TestClass]
    public class TodoServiceTest {

        private const int Total = 55;

        private static PagedResult<Todo> Get(int page, int pageSize, string sortBy = null, bool descending = false) {
            return new TodoService().List(new PageRequest {
                Page = page, PageSize = pageSize, SortBy = sortBy, Descending = descending
            });
        }

        [TestMethod]
        public void DefaultRequest_Returns20ItemsNewestFirst() {
            var result = new TodoService().List(new PageRequest());

            Assert.AreEqual(20, result.Items.Count);
            Assert.AreEqual(1, result.Page);
            Assert.AreEqual(20, result.PageSize);
            Assert.AreEqual(Total, result.TotalItems);
            Assert.AreEqual(3, result.TotalPages);
            Assert.AreEqual(Total, result.Items[0].Id); // createdDate desc => the latest todo comes first
            CollectionAssertSorted(result.Items.Select(t => t.CreatedDate), descending: true);
        }

        [TestMethod]
        public void LastPage_ContainsOnlyTheRemainingItems() {
            var result = Get(3, 20);
            Assert.AreEqual(15, result.Items.Count); // 55 = 20 + 20 + 15
            Assert.AreEqual(3, result.Page);
        }

        [TestMethod]
        public void PageSize10_Gives6Pages() {
            Assert.AreEqual(6, Get(1, 10).TotalPages);
            Assert.AreEqual(5, Get(6, 10).Items.Count);
        }

        [TestMethod]
        public void PageSizeAll_ReturnsEverythingInOnePage() {
            var result = Get(5, PageRequest.All); // page is ignored
            Assert.AreEqual(Total, result.Items.Count);
            Assert.AreEqual(1, result.Page);
            Assert.AreEqual(1, result.TotalPages);
        }

        [TestMethod]
        public void PageOutOfRange_IsClamped() {
            Assert.AreEqual(3, Get(99, 20).Page);
            Assert.AreEqual(15, Get(99, 20).Items.Count);
            Assert.AreEqual(1, Get(0, 20).Page);
            Assert.AreEqual(1, Get(-4, 20).Page);
        }

        [TestMethod]
        public void SortByTask_PagesNeverOverlapAndCoverEverything() {
            var ids = new List<long>();
            for (var page = 1; page <= 6; page++) {
                ids.AddRange(Get(page, 10, "task").Items.Select(t => t.Id));
            }
            Assert.AreEqual(Total, ids.Count);
            Assert.AreEqual(Total, ids.Distinct().Count());
        }

        [TestMethod]
        public void SortById_RespectsDirectionAndIsCaseInsensitive() {
            Assert.AreEqual(1, Get(1, 10, "id", descending: false).Items[0].Id);
            Assert.AreEqual(Total, Get(1, 10, "ID", descending: true).Items[0].Id);
        }

        [TestMethod]
        public void SortIsAppliedToTheWholeList_NotOnlyToThePage() {
            // Alphabetically the smallest task of ALL items is "... 1" and the biggest is "... 9" (not 55, the newest id), 
            // so page 1 only starts with them if the sort ran on the whole list before slicing.
            Assert.AreEqual("Dont forget to do 1", Get(1, 10, "task").Items[0].Task);
            Assert.AreEqual("Dont forget to do 9", Get(1, 10, "task", descending: true).Items[0].Task);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UnknownSortProperty_IsRejected() {
            Get(1, 10, "password");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NegativePageSize_IsRejected() {
            Get(1, -1);
        }

        private static void CollectionAssertSorted(IEnumerable<DateTime> values, bool descending) {
            var list = values.ToList();
            var expected = descending ? list.OrderByDescending(d => d) : list.OrderBy(d => d);
            Assert.IsTrue(expected.SequenceEqual(list), "list is not sorted");
        }

    }
}
