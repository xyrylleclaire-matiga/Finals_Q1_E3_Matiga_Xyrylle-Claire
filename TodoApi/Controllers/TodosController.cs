using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using System.Collections.Concurrent;

namespace TodoApi.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class TodosController : ControllerBase {
        private static List<Todo> _todos = new();
        private static readonly object _lock = new();

        [HttpGet]
        public IActionResult Get() {
            lock(_lock) {
                return Ok(_todos.ToList());
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] Todo newTodo) {
            if (string.IsNullOrWhiteSpace(newTodo.Title)) {
                return BadRequest("Title is required.");
            }

            newTodo.Id = Guid.NewGuid();
            lock(_lock) {
                _todos.Add(newTodo);
            }
            return CreatedAtAction(nameof(Get), new { id = newTodo.Id }, newTodo);
        }

        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] Todo updatedTodo) {
            lock(_lock) {
                var todo = _todos.FirstOrDefault(t => t.Id == id);
                if (todo == null) return NotFound();

                if (string.IsNullOrWhiteSpace(updatedTodo.Title)) {
                    return BadRequest("Title is required.");
                }

                todo.Title = updatedTodo.Title;
                todo.Completed = updatedTodo.Completed;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id) {
            lock(_lock) {
                var todo = _todos.FirstOrDefault(t => t.Id == id);
                if (todo == null) return NotFound();

                _todos.Remove(todo);
            }
            return NoContent();
        }
    }
}
