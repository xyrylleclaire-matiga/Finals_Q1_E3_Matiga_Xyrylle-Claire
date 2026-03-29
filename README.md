<<<<<<< HEAD
# Finals_Q1 - Todo Management System API

This is the backend for the Todo Management System, built with .NET 9 Web API.

## Features
- **CRUD Operations**: Manage todos via a RESTful API.
- **In-Memory Storage**: Uses a thread-safe `List<Todo>` for storage.
- **CORS Support**: Configured to allow requests from `http://localhost:5173`.
- **Validation**: Rejects empty `Title` values with `400 Bad Request`.

## API Endpoints
- `GET /api/todos`: Retrieve all todos.
- `POST /api/todos`: Create a new todo.
  - Body: `{ "title": "string", "completed": boolean }`
- `PUT /api/todos/{id}`: Update an existing todo.
  - Body: `{ "title": "string", "completed": boolean }`
- `DELETE /api/todos/{id}`: Delete a todo.

## Setup & Execution
1. Ensure you have the .NET SDK installed.
2. Navigate to the `TodoApi` directory:
   ```bash
   cd TodoApi
   ```
3. Run the application:
   ```bash
   dotnet run
   ```
4. The API will be available at `http://localhost:5000` (or the port specified in `launchSettings.json`).

## Architectural Patterns
- **Repository Pattern (Simplified)**: Data is managed within the controller for this assignment.
- **RESTful Principles**: Uses standard HTTP verbs and status codes (200, 201, 204, 400).
- **Concurrency**: Thread-safe operations using `lock` on the in-memory list.
=======
# Finals_Q1_E3_Matiga_Xyrylle-Claire
>>>>>>> c4312d42d71fba9d37dc46e2e3c08720253b634a
