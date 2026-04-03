using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace TodoApi.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class TodosController : ControllerBase {
        private static List<Todo> _todos = new();
        private static readonly object _lock = new();

        private string ComputeHash(Todo todo) {
            using (SHA256 sha256Hash = SHA256.Create()) {
                string rawData = $"{todo.Id}{todo.Title}{todo.Completed}{todo.PreviousHash}";
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes) {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private void RecalculateChain(int startIndex = 0) {
            for (int i = startIndex; i < _todos.Count; i++) {
                _todos[i].PreviousHash = i == 0 ? "0" : _todos[i - 1].Hash;
                _todos[i].Hash = ComputeHash(_todos[i]);
            }
        }

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
                newTodo.PreviousHash = _todos.Count == 0 ? "0" : _todos.Last().Hash;
                newTodo.Hash = ComputeHash(newTodo);
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
                int idx = _todos.IndexOf(todo);
                RecalculateChain(idx);
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id) {
            lock(_lock) {
                var todo = _todos.FirstOrDefault(t => t.Id == id);
                if (todo == null) return NotFound();

                _todos.Remove(todo);
                RecalculateChain(0);
            }
            return NoContent();
        }

        [HttpGet("verify")]
        public IActionResult Verify() {
            lock(_lock) {
                for (int i = 0; i < _todos.Count; i++) {
                    var current = _todos[i];
                    var expectedPrevious = i == 0 ? "0" : _todos[i - 1].Hash;
                    if (current.PreviousHash != expectedPrevious) return Conflict("Chain Tampered: PreviousHash mismatch");
                    if (current.Hash != ComputeHash(current)) return Conflict("Chain Tampered: Data mutation detected");
                }
                return Ok(new { valid = true, length = _todos.Count });
            }
        }
        
        // HACK ENDPOINT TO SIMULATE TAMPERING FOR TESTING
        [HttpPost("hack/{id}")]
        public IActionResult Hack(Guid id) {
            lock(_lock) {
                var todo = _todos.FirstOrDefault(t => t.Id == id);
                if (todo == null) return NotFound();
                todo.Title = "HACKED DATA";
                // Intentionally skipping recalculation to break the blockchain hash integrity
                return Ok("Tampered successfully.");
            }
        }
    }
}
