using System;
using System.Web.Http;
using InterviewTestPagination.Models;
using InterviewTestPagination.Models.Todo;

namespace InterviewTestPagination.Controllers
{
    /// <summary>
    /// 'Rest' controller for the <see cref="Todo"/>
    /// model.
    /// 
    /// GET api/Todo/Todos?page=1&amp;pageSize=20&amp;sortBy=createdDate&amp;sortDir=desc
    ///   page      1
    ///   pageSize  items per page; 0 = all 
    ///   sortBy    id | task | createdDate
    ///   sortDir   asc | desc 
    /// Response: { items: [...], page, pageSize, totalItems, totalPages }
    /// </summary>
    [RoutePrefix("api/Todo")]
    public class TodoController : ApiController
    {

        // TODO

        private readonly IModelService<Todo> _todoService = new TodoService();

        // Rota de atributo explícito: a rota padrão "api/{controller}/{id}" só correspondia à antiga
        // URL "api/Todo/Todos" por engano (vinculou "Todos" a {id}).
        [HttpGet]
        [Route("Todos")]
        public IHttpActionResult Todos(
            int page = 1,
            int pageSize = PageRequest.DefaultPageSize,
            string sortBy = null,
            string sortDir = "asc")
        {

            bool descending;
            if (string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase))
            {
                descending = true;
            }
            else if (string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase))
            {
                descending = false;
            }
            else
            {
                return BadRequest("sortDir must be 'asc' or 'desc'.");
            }

            try
            {
                var result = _todoService.List(new PageRequest
                {
                    Page = page,
                    PageSize = pageSize,
                    SortBy = sortBy,
                    Descending = descending
                });
                return Ok(result);
            }
            catch (ArgumentException e)
            {
                // invalid pageSize (400)
                return BadRequest(e.Message);
            }
        }

    }
}
