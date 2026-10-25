using System.Collections.Generic;

namespace InterviewTestPagination.Models {

    /// <summary>
    /// Datasource/Driver layer's main entry-point.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IModelRepository<T> {
        /// <summary>
        /// Lists all entries of type T (default ordering).
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> All();

        /// <summary>
     /// Retorna a quantidade total de registros disponíveis
        /// </summary>
        int Count();

        /// Retorna uma lista de registros aplicando ordenação e paginação.
        IEnumerable<T> List(string sortBy, bool descending, int skip, int? take);
    }
}
