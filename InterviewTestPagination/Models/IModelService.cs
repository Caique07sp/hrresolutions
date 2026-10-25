using System.Collections.Generic;

namespace InterviewTestPagination.Models {
    /// <summary>
    /// Model Service layer's main entry-point. 
    /// Should translate high-level commands and data structures into something that can be used by the data source layer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IModelService<T> {

        /// <summary>
        /// Reference to Repository layer
        /// </summary>
        IModelRepository<T> Repository { get; set; }

        /// <summary>
        /// Lists all entries of model T (no paging).
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> List();

        /// <summary>
        /// Lists one sorted page of entries of model T, together with the paging metadata.
        /// </summary>
        /// <exception cref="System.ArgumentException">for an invalid page size or sort property</exception>
        PagedResult<T> List(PageRequest request);
    }
}
