using System.Collections.Generic;

namespace InterviewTestPagination.Models {

    /// <summary>
    /// Service-layer/API response DTO: one page of items plus everything the client 
    /// needs to render a pagination component. Values are the *effective* ones (after clamping), 
    /// so the client can simply trust them.
    /// </summary>
    public class PagedResult<T> {

        /// <summary>Items of the current page, already sorted.</summary>
        public IList<T> Items { get; set; }

        /// <summary>Effective 1-based page number.</summary>
        public int Page { get; set; }

        /// <summary>Effective page size (0 means "all").</summary>
        public int PageSize { get; set; }

        /// <summary>Total number of items in the whole list (not only in this page).</summary>
        public int TotalItems { get; set; }

        /// <summary>Total number of pages (at least 1, even when the list is empty).</summary>
        public int TotalPages { get; set; }
    }
}
