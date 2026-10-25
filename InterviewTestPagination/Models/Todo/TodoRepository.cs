using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace InterviewTestPagination.Models.Todo {

    /// <summary>
    /// No need to use an actual persistent datasource. 
    /// All operations can be mocked in-memory as long as they are consistent with the chosen datasource implementation 
    /// (e.g. dont create new model instances when executing a search 'query', etc).
    /// TL;DR: from this point on Database-like operations can be mocked.
    /// </summary>
    public class TodoRepository : IModelRepository<Todo> {

        /// <summary>
        /// Example in-memory model datasource 'indexed' by id.
        /// </summary>
        private static readonly IDictionary<long, Todo> DataSource = new ConcurrentDictionary<long, Todo>();

        /// <summary>
        /// Whitelist of sortable properties (the 'columns' of this datasource). 
        /// Never sort through reflection on a client-provided name: anything not listed here is rejected.
        /// </summary>
        private static readonly IDictionary<string, Func<Todo, IComparable>> SortKeys =
            new Dictionary<string, Func<Todo, IComparable>>(StringComparer.OrdinalIgnoreCase) {
                { "id", t => t.Id },
                { "task", t => t.Task },
                { "createdDate", t => t.CreatedDate }
            };

        static TodoRepository() {
            // initializing datasource
            var startDate = DateTime.Today;
            for (var i = 1; i <= 55; i++) {
                var createdDate = startDate.AddDays(i);
                DataSource[i] = new Todo(id: i, task: "Dont forget to do " + i, createdDate: createdDate);
            }
        }

        public IEnumerable<Todo> All() {
            return DataSource.Values.OrderByDescending(t => t.CreatedDate);
        }

        public int Count() {
            return DataSource.Count;
        }

        public IEnumerable<Todo> List(string sortBy, bool descending, int skip, int? take) {
            // default ordering (same as All()): newest first
            if (string.IsNullOrWhiteSpace(sortBy)) {
                sortBy = "createdDate";
                descending = true;
            }

            Func<Todo, IComparable> key;
            if (!SortKeys.TryGetValue(sortBy.Trim(), out key)) {
                throw new ArgumentException(
                    "Cannot sort by '" + sortBy + "'. Sortable properties: " + string.Join(", ", SortKeys.Keys) + ".", "sortBy");
            }

            // ThenBy(Id) is the tie-breaker: without a total order, equal keys could swap places between 
            // two requests and the same item would show up on two pages (or on none).
            var sorted = descending ? DataSource.Values.OrderByDescending(key) : DataSource.Values.OrderBy(key);
            var query = sorted.ThenBy(t => t.Id).Skip(skip);
            if (take.HasValue) {
                query = query.Take(take.Value);
            }
            return query.ToList();
        }

    }
}
