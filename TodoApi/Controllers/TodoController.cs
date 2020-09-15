using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Produces("application/json")] // declare that the controller's actions support a response content type of application/json:
    [Route("api/[controller]")]
    [ApiController]
    //批注 Web API 控制器类: 自动 HTTP 400 响应
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;

            if (!_context.TodoItems.Any())
            {
                _context.TodoItems.Add(new TodoItem {Name = "Item1"});
                _context.SaveChanges();
            }
        }

        
        [HttpGet]
        public ActionResult<List<TodoItem>> GetAll()
        {
            //Get All Todo’s
            return _context.TodoItems.ToList();
        }

        
        [HttpGet("{id}", Name = "GetTodo")]
        public ActionResult<TodoItem> GetById(long id)
        {
            //Get Specific Todo
            var item = _context.TodoItems.Find(id);
            if (item == null)
            {
                return NotFound();
            }

            return item;
        }

        [HttpGet("{id}")]
        public ActionResult<TodoItem> GetIncomingToDo(long id)
        {
            //Get Incoming ToDo (for today/next day/current week)
            //id = 1 = Today
            //id = 2 = next day
            //id = 3 = current week

            TodoItem item = null;

            if (id == 1) 
            {
                item = (TodoItem)_context.TodoItems.Where(p => p.Expiry.Date == DateTime.Now.Date);
            }
            else if (id == 2)
            {
                item = (TodoItem)_context.TodoItems.Where(p => p.Expiry.Date == DateTime.Now.Date.AddDays(1));
            }
            else if (id == 3)
            {
                DateTime startOfWeek = DateTime.Today.AddDays(-1 * (int)(DateTime.Today.DayOfWeek));
                item = (TodoItem)_context.TodoItems.Where(p => p.Expiry.Date >= startOfWeek.Date
                  && p.Expiry.Date <= startOfWeek.Date.AddDays(6));
            }

            if (item == null)
            {
                return NotFound();
            }

            return item;
        }

        /// <summary>
        /// Creates a TodoItem.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Todo
        ///     {
        ///        "id": 1,
        ///        "name": "Item1",
        ///        "isComplete": true
        ///     }
        ///
        /// </remarks>
        /// <param name="item"> todoItem </param>
        /// <returns>A newly created TodoItem</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>            
        [ProducesResponseType(201)] //restrict show response http type, xml show http type indroduction
        [ProducesResponseType(400)]
        [HttpPost]
        public ActionResult Create(TodoItem item)
        {
            //Create Todo
            _context.TodoItems.Add(item);
            _context.SaveChanges();

            return CreatedAtRoute("GetTodo", new {id = item.Id}, item);
        }

        [HttpPut("{id}")]
        public ActionResult Update(long id, TodoItem item)
        {
            //Update Todo
            var todo = _context.TodoItems.Find(id);

            if (todo == null)
            {
                return NotFound();
            }

            todo.IsComplete = item.IsComplete;
            todo.Expiry = item.Expiry;
            todo.Description = item.Description;
            todo.PercentComplete = item.PercentComplete;
            todo.Name = item.Name;

            _context.TodoItems.Update(todo);
            _context.SaveChanges();
            return NoContent(); //HTTP的204(No Content)响应, 就表示执行成功, 但是没有数据, 浏览器不用刷新页面.也不用导向新的页面.
        }

        [HttpPut("{id}")]
        public ActionResult UpdatePercentComplete(long id, TodoItem item)
        {
            //Set Todo percent complete
            var todo = _context.TodoItems.Find(id);

            if (todo == null)
            {
                return NotFound();
            }

           todo.PercentComplete = item.PercentComplete;

            _context.TodoItems.Update(todo);
            _context.SaveChanges();
            return NoContent(); //HTTP的204(No Content)响应, 就表示执行成功, 但是没有数据, 浏览器不用刷新页面.也不用导向新的页面.
        }
        [HttpPut("{id}")]
        public ActionResult UpdateDone(long id, TodoItem item)
        {
            //Mark Todo as Done
            var todo = _context.TodoItems.Find(id);

            if (todo == null)
            {
                return NotFound();
            }

            todo.IsComplete = true;

            _context.TodoItems.Update(todo);
            _context.SaveChanges();
            return NoContent(); //HTTP的204(No Content)响应, 就表示执行成功, 但是没有数据, 浏览器不用刷新页面.也不用导向新的页面.
        }

        /// <summary>
        /// Deletes a specific TodoItem.
        /// </summary>
        /// <param name="id"> item id </param> 
        [HttpDelete("{id}")]
        public ActionResult Delete(long id)
        {
            //Delete Todo
            var todo = _context.TodoItems.Find(id);
           
            if (todo == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todo);
            _context.SaveChanges();
            return NoContent();
        }
    }
}