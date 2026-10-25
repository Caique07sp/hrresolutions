namespace InterviewTestPagination.Models {

    /// <summary>
    /// Service-layer DTO describing which slice of a list the caller wants.
    /// It is model-agnostic, so it can be reused by any <see cref="IModelService{T}"/>.
    /// </summary>
    public class PageRequest {

        /// <summary>Page size used when the client does not specify one.</summary>
        public const int DefaultPageSize = 20;

        /// <summary>Special <see cref="PageSize"/> value meaning "disregard pagination, return everything".</summary>
        public const int All = 0;

        /// <summary>1-based page number. Out-of-range values are clamped by the service.</summary>
        public int Page { get; set; } = 1;

        /// <summary>Items per page, or <see cref="All"/> (0). Negative values are rejected.</summary>
        public int PageSize { get; set; } = DefaultPageSize;

        /// <summary>
        /// Name of the property to sort by (case-insensitive). 
        /// When null/empty the model's default ordering is used and <see cref="Descending"/> is ignored.
        /// </summary>
        public string SortBy { get; set; }

        /// <summary>True for descending order.</summary>
        public bool Descending { get; set; }
    }
}
