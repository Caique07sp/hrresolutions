using System;
using System.Collections.Generic;

namespace InterviewTestPagination.Models.Todo {
    /// <summary>
    /// Todo service: translates a <see cref="PageRequest"/> into repository calls and builds the <see cref="PagedResult{T}"/>.
    /// </summary>
    public class TodoService : IModelService<Todo> {

        private IModelRepository<Todo> _repository = new TodoRepository();

        public IModelRepository<Todo> Repository {
            get { return _repository; }
            set { _repository = value; }
        }

        /// <summary>
        /// Lists all entries of type <see cref="Todo"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Todo> List() {
            // invoke Datasource layer
            return Repository.All();
        }

        /// <summary>
        /// Lists one page of <see cref="Todo"/>s.
        /// 
        /// Forgiving with the page number (the list may shrink between requests, so a stale page number is not an error): 
        /// it is clamped to [1, totalPages]. Strict with things that are really bugs in the caller: 
        /// negative page size and unknown sort properties throw <see cref="ArgumentException"/>.
        /// </summary>
        public PagedResult<Todo> List(PageRequest request) {
            if (request == null) {
                request = new PageRequest();
            }
            if (request.PageSize < 0) {
                throw new ArgumentOutOfRangeException("request", "PageSize cannot be negative (use 0 for 'all').");
            }

            var all = request.PageSize == PageRequest.All;
            var totalItems = Repository.Count();

            // integer ceiling division; an empty list still has 1 (empty) page so the UI can say "page 1 of 1"
            var totalPages = all || totalItems == 0 ? 1 : (totalItems + request.PageSize - 1) / request.PageSize;
            var page = Math.Min(Math.Max(request.Page, 1), totalPages);

            var skip = all ? 0 : (page - 1) * request.PageSize;
            int? take = all ? (int?)null : request.PageSize;

            return new PagedResult<Todo> {
                Items = new List<Todo>(Repository.List(request.SortBy, request.Descending, skip, take)),
                Page = page,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }
    }
}
