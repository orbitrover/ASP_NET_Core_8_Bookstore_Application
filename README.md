# ASP_NET_Core_8_Bookstore_Application
Let's go through the steps to set up the ASP.NET Core 8 bookstore application using clean architecture and the associated unit test project.
1. Create the ASP.NET Core 8 Bookstore Application
1.1 Project Structure
We'll use the Clean Architecture approach, which typically divides the solution into the following projects:

Application Layer: Contains business logic and interfaces.
Domain Layer: Contains entities and core logic.
Infrastructure Layer: Contains data access and external service integrations.
WebUI Layer: Contains the ASP.NET Core 8 MVC or API project.
Test Layer: Contains unit tests.
1.2 Setting Up the Project
Create the Solution:

bash
Copy code
dotnet new sln -n BookstoreApp
Create Projects:

bash
Copy code
# Create the Domain project
dotnet new classlib -n BookstoreApp.Domain
dotnet sln add ./BookstoreApp.Domain/BookstoreApp.Domain.csproj

# Create the Application project
dotnet new classlib -n BookstoreApp.Application
dotnet sln add ./BookstoreApp.Application/BookstoreApp.Application.csproj
dotnet add ./BookstoreApp.Application/BookstoreApp.Application.csproj reference ./BookstoreApp.Domain/BookstoreApp.Domain.csproj

# Create the Infrastructure project
dotnet new classlib -n BookstoreApp.Infrastructure
dotnet sln add ./BookstoreApp.Infrastructure/BookstoreApp.Infrastructure.csproj
dotnet add ./BookstoreApp.Infrastructure/BookstoreApp.Infrastructure.csproj reference ./BookstoreApp.Application/BookstoreApp.Application.csproj

# Create the WebUI project
dotnet new webapi -n BookstoreApp.WebUI
dotnet sln add ./BookstoreApp.WebUI/BookstoreApp.WebUI.csproj
dotnet add ./BookstoreApp.WebUI/BookstoreApp.WebUI.csproj reference ./BookstoreApp.Infrastructure/BookstoreApp.Infrastructure.csproj

# Create the Test project
dotnet new xunit -n BookstoreApp.Tests
dotnet sln add ./BookstoreApp.Tests/BookstoreApp.Tests.csproj
dotnet add ./BookstoreApp.Tests/BookstoreApp.Tests.csproj reference ./BookstoreApp.Application/BookstoreApp.Application.csproj
2. Implementing the Bookstore Domain
2.1 Create the Book Entity
In the BookstoreApp.Domain project, create a new class Book.cs:

csharp
Copy code
namespace BookstoreApp.Domain.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
    }
}
3. Implementing the Application Layer
3.1 Create Interfaces
In BookstoreApp.Application, create an interface IBookService.cs:

csharp
Copy code
using BookstoreApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookstoreApp.Application.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetBooksAsync();
        Task<Book> GetBookByIdAsync(int id);
        Task AddBookAsync(Book book);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(int id);
    }
}
3.2 Implement the Service
In BookstoreApp.Application, create a class BookService.cs:

csharp
Copy code
using BookstoreApp.Application.Interfaces;
using BookstoreApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookstoreApp.Application.Services
{
    public class BookService : IBookService
    {
        private readonly List<Book> _books = new();

        public Task<IEnumerable<Book>> GetBooksAsync()
        {
            return Task.FromResult((IEnumerable<Book>)_books);
        }

        public Task<Book> GetBookByIdAsync(int id)
        {
            var book = _books.Find(b => b.Id == id);
            return Task.FromResult(book);
        }

        public Task AddBookAsync(Book book)
        {
            _books.Add(book);
            return Task.CompletedTask;
        }

        public Task UpdateBookAsync(Book book)
        {
            var existingBook = _books.Find(b => b.Id == book.Id);
            if (existingBook != null)
            {
                existingBook.Title = book.Title;
                existingBook.Author = book.Author;
                existingBook.Price = book.Price;
            }
            return Task.CompletedTask;
        }

        public Task DeleteBookAsync(int id)
        {
            var book = _books.Find(b => b.Id == id);
            if (book != null)
            {
                _books.Remove(book);
            }
            return Task.CompletedTask;
        }
    }
}
4. Implementing the WebUI Layer
4.1 Setup Dependency Injection
In BookstoreApp.WebUI, modify Program.cs:

csharp
Copy code
using BookstoreApp.Application.Interfaces;
using BookstoreApp.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the BookService
builder.Services.AddScoped<IBookService, BookService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
4.2 Create Book Controller
In BookstoreApp.WebUI, create a BooksController.cs:

csharp
Copy code
using BookstoreApp.Application.Interfaces;
using BookstoreApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreApp.WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks()
        {
            var books = await _bookService.GetBooksAsync();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] Book book)
        {
            await _bookService.AddBookAsync(book);
            return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            await _bookService.UpdateBookAsync(book);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            await _bookService.DeleteBookAsync(id);
            return NoContent();
        }
    }
}
5. Implementing Unit Tests
5.1 Setup Unit Test for BookService
In BookstoreApp.Tests, create a class BookServiceTests.cs:

csharp
Copy code
using BookstoreApp.Application.Interfaces;
using BookstoreApp.Application.Services;
using BookstoreApp.Domain.Entities;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BookstoreApp.Tests
{
    public class BookServiceTests
    {
        private readonly IBookService _bookService;

        public BookServiceTests()
        {
            _bookService = new BookService();
        }

        [Fact]
        public async Task Can_Add_Book()
        {
            var book = new Book { Id = 1, Title = "Sample Book", Author = "Author", Price = 9.99m };

            await _bookService.AddBookAsync(book);
            var books = await _bookService.GetBooksAsync();

            Assert.Contains(books, b => b.Id == 1);
        }

        [Fact]
        public async Task Can_Get_Book_By_Id()
        {
            var book = new Book { Id = 1, Title = "Sample Book", Author = "Author", Price = 9.99m };

            await _bookService.AddBookAsync(book);
            var retrievedBook = await _bookService.GetBookByIdAsync(1);

            Assert.NotNull(retrievedBook);
            Assert.Equal("Sample Book", retrievedBook.Title);
        }

        [Fact]
        public async Task Can_Update_Book()
        {
            var book = new Book { Id = 1, Title = "Sample Book", Author = "Author", Price = 9.99m };

            await _bookService.AddBookAsync(book);

            book.Title = "Updated Book";
            await _bookService.UpdateBookAsync(book);

            var updatedBook = await _bookService.GetBookByIdAsync(1);
            Assert.Equal("Updated Book", updatedBook.Title);
        }

        [Fact]
        public async Task Can_Delete_Book()
        {
            var book = new Book { Id = 1, Title = "Sample Book", Author = "Author", Price = 9.99m };

            await _bookService.AddBookAsync(book);
            await _bookService.DeleteBookAsync(1);

            var books = await _bookService.GetBooksAsync();
            Assert.DoesNotContain(books, b => b.Id == 1);
        }
    }
}
6. Running the Application and Tests
Run the Web Application: You can start the WebAPI by running the BookstoreApp.WebUI project.
Run the Tests: Use the following command to run the unit tests:
bash
Copy code
dotnet test
7. Integrating with CI/CD
Now, with the code and tests in place, you can use the GitHub Actions CI/CD pipeline setup I provided earlier to automate building, testing, and deploying this application.

This complete setup should help you get started with a basic ASP.NET Core 8 bookstore application based on clean architecture, and also includes unit tests to ensure the reliability of your business logic.

The Infrastructure Layer in a Clean Architecture setup is responsible for implementing the details of data access, external service integrations, and other cross-cutting concerns like logging, caching, or sending emails. This layer typically includes the actual implementations of interfaces defined in the Application Layer and might interact with external systems like databases, file systems, or third-party APIs.

Why Use an Infrastructure Layer?
Separation of Concerns: Keeps the core business logic (in the Application Layer) free from infrastructure-related concerns like databases or external APIs.
Flexibility: Allows you to easily switch out infrastructure components without affecting the business logic. For example, you can change the database or the way you log information without modifying the core logic.
Testability: By keeping infrastructure concerns separate, it's easier to mock dependencies and write unit tests for the business logic.
Implementing the Infrastructure Layer
Let's add the necessary code to the Infrastructure Layer for our bookstore application.

1. Creating the Data Access Implementation
In BookstoreApp.Infrastructure, let's create a class that implements IBookService using an in-memory database or a real database (like Entity Framework Core).

1.1 Setting Up a Database Context (using Entity Framework Core)
First, you need to install the required NuGet packages for Entity Framework Core.

bash
Copy code
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.Extensions.Logging.Console
Next, create a BookstoreDbContext class in BookstoreApp.Infrastructure:

csharp
Copy code
using BookstoreApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookstoreApp.Infrastructure.Persistence
{
    public class BookstoreDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }

        public BookstoreDbContext(DbContextOptions<BookstoreDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // You can configure your entity properties here if needed
            modelBuilder.Entity<Book>().HasKey(b => b.Id);
        }
    }
}
1.2 Implementing IBookService with Entity Framework Core
Now, let's implement the IBookService in the BookstoreApp.Infrastructure project:

csharp
Copy code
using BookstoreApp.Application.Interfaces;
using BookstoreApp.Domain.Entities;
using BookstoreApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookstoreApp.Infrastructure.Services
{
    public class BookService : IBookService
    {
        private readonly BookstoreDbContext _context;

        public BookService(BookstoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetBooksAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<Book> GetBookByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task AddBookAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBookAsync(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }
    }
}
2. Registering Infrastructure Services
You need to configure the services in the BookstoreApp.WebUI project to use the BookService implementation from the Infrastructure Layer. Modify the Program.cs:

```csharp
using BookstoreApp.Application.Interfaces;
using BookstoreApp.Infrastructure.Persistence;
using BookstoreApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the BookRepository
builder.Services.AddScoped<IBookRepository, BookRepository>();
// Register the Static BookService
builder.Services.AddSingleton<IBookService, BookService>();
// Register the BookService from the Infrastructure Layer
builder.Services.AddScoped<IBookService, BookServiceDB>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
```
3. Update the Configuration
Make sure you have a connection string for your database in appsettings.json:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BookstoreDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```
4. Running Migrations
You can now create and apply migrations for your database.

```bash
dotnet ef migrations add InitialCreate -p BookstoreApp.Infrastructure -s BookstoreApp.WebUI -o Persistence/Migrations
dotnet ef database update -p BookstoreApp.Infrastructure -s BookstoreApp.WebUI
```
5. Summary
Infrastructure Layer: Provides the concrete implementations for data access and external integrations, keeping the core business logic decoupled from these concerns.
Entity Framework Core: Used here to implement data access for BookService, but you could replace this with any other data access method if needed.
Dependency Injection: The WebUI layer is configured to use the services from the Infrastructure Layer.
This setup completes the Clean Architecture implementation by separating concerns and allowing the application to be scalable, maintainable, and testable.

To add an MVC layer (BookstoreApp.Web) that consumes the BookstoreApp.WebUI API, you'll need to create a new project within your solution and set it up to interact with the WebAPI.

1. Create the MVC Project
Add the MVC Project:

```bash
dotnet new mvc -n BookstoreApp.Web
dotnet sln add ./BookstoreApp.Web/BookstoreApp.Web.csproj
```
Set Up Dependencies:

Add a reference to the BookstoreApp.Application project (if needed for shared models).
Install the HttpClient library if it's not already available.
bash
Copy code
dotnet add package Microsoft.Extensions.Http
2. Set Up the MVC Project
2.1 Configure the MVC Project to Use HttpClient
In BookstoreApp.Web, modify Program.cs to register HttpClient:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register HttpClient for API calls
builder.Services.AddHttpClient("BookstoreApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7150/api/"); // Update this URL to match your WebAPI's URL
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```
2.2 Create Models (if needed)
If you need models that match the WebAPI's data structures, you can create them in the Models folder in the BookstoreApp.Web project:

```csharp
namespace BookstoreApp.Web.Models
{
    public class BookViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
    }
}
```
3. Create Controllers and Views
3.1 BooksController
Create a BooksController in the Controllers folder:

```csharp
Copy code
using BookstoreApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BookstoreApp.Web.Controllers
{
      public class BooksController : Controller
      {
          private readonly IHttpClientFactory _httpClientFactory;
          private readonly HttpClient _httpClient;
          public BooksController(IHttpClientFactory httpClientFactory)
          {
              _httpClientFactory = httpClientFactory;
              _httpClient = _httpClientFactory.CreateClient("BookstoreApiClient");
          }
          
          public async Task<IActionResult> Index()
          {
              try
              {
                  var response = await _httpClient.GetAsync("books");
          
                  if (response.IsSuccessStatusCode)
                  {
                      var responseData = await response.Content.ReadAsStringAsync();
                      var books = JsonConvert.DeserializeObject<IEnumerable<BookViewModel>>(responseData);
                      return View(books);
                  }
              }
              catch(Exception ex)
              {
                  throw ex;
              }
              return View("Error");
          }
          
          public async Task<IActionResult> Details(int id)
          {
             
              var response = await _httpClient.GetAsync($"books/{id}");
          
              if (response.IsSuccessStatusCode)
              {
                  var responseData = await response.Content.ReadAsStringAsync();
                  var book = JsonConvert.DeserializeObject<BookViewModel>(responseData);
                  return View(book);
              }
          
              return View("Error");
          }
          
          [HttpGet]
          public IActionResult Create()
          {
              return View();
          }
          
          [HttpPost]
          public async Task<IActionResult> Create(BookViewModel model)
          {
              var response = await _httpClient.PostAsJsonAsync("books", model);
          
              if (response.IsSuccessStatusCode)
              {
                  return RedirectToAction(nameof(Index));
              }
          
              return View("Error");
          }
          
          [HttpGet]
          public async Task<IActionResult> Edit(int id)
          {
              var response = await _httpClient.GetAsync($"books/{id}");
          
              if (response.IsSuccessStatusCode)
              {
                  var responseData = await response.Content.ReadAsStringAsync();
                  var book = JsonConvert.DeserializeObject<BookViewModel>(responseData);
                  return View(book);
              }
          
              return View("Error");
          }
          
          [HttpPost]
          public async Task<IActionResult> Edit(BookViewModel model)
          {
              var response = await _httpClient.PutAsJsonAsync($"books/{model.Id}", model);
          
              if (response.IsSuccessStatusCode)
              {
                  return RedirectToAction(nameof(Index));
              }
          
              return View("Error");
          }
          
          [HttpGet]
          public async Task<IActionResult> Delete(int id)
          {
              var response = await _httpClient.GetAsync($"books/{id}");
          
              if (response.IsSuccessStatusCode)
              {
                  var responseData = await response.Content.ReadAsStringAsync();
                  var book = JsonConvert.DeserializeObject<BookViewModel>(responseData);
                  return View(book);
              }
          
              return View("Error");
          }
          
          [HttpPost, ActionName("Delete")]
          public async Task<IActionResult> DeleteConfirmed(int id)
          {
              var response = await _httpClient.DeleteAsync($"books/{id}");
          
              if (response.IsSuccessStatusCode)
              {
                  return RedirectToAction(nameof(Index));
              }
          
              return View("Error");
          }
      }
}
```

3.2 Create Views
Create the corresponding Razor views in the Views/Books folder:

Index.cshtml:

html
Copy code
@model IEnumerable<BookstoreApp.Web.Models.BookViewModel>

<h1>Books</h1>
<p>
    <a asp-action="Create">Create New</a>
</p>

<table class="table">
    <thead>
        <tr>
            <th>Title</th>
            <th>Author</th>
            <th>Price</th>
            <th></th>
        </tr>
    </thead>
    <tbody>
        @foreach (var book in Model)
        {
            <tr>
                <td>@book.Title</td>
                <td>@book.Author</td>
                <td>@book.Price</td>
                <td>
                    <a asp-action="Details" asp-route-id="@book.Id">Details</a> |
                    <a asp-action="Edit" asp-route-id="@book.Id">Edit</a> |
                    <a asp-action="Delete" asp-route-id="@book.Id">Delete</a>
                </td>
            </tr>
        }
    </tbody>
</table>
Details.cshtml:

html
Copy code
@model BookstoreApp.Web.Models.BookViewModel

<h1>Book Details</h1>
<dl class="row">
    <dt class="col-sm-2">Title</dt>
    <dd class="col-sm-10">@Model.Title</dd>

    <dt class="col-sm-2">Author</dt>
    <dd class="col-sm-10">@Model.Author</dd>

    <dt class="col-sm-2">Price</dt>
    <dd class="col-sm-10">@Model.Price</dd>
</dl>
<p>
    <a asp-action="Edit" asp-route-id="@Model.Id">Edit</a> |
    <a asp-action="Index">Back to List</a>
</p>
Create.cshtml and Edit.cshtml:

html
Copy code
@model BookstoreApp.Web.Models.BookViewModel

<h1>@ViewData["Title"]</h1>

<form asp-action="@ViewData["Action"]">
    <div class="form-group">
        <label asp-for="Title" class="control-label"></label>
        <input asp-for="Title" class="form-control" />
        <span asp-validation-for="Title" class="text-danger"></span>
    </div>
    <div class="form-group">
        <label asp-for="Author" class="control-label"></label>
        <input asp-for="Author" class="form-control" />
        <span asp-validation-for="Author" class="text-danger"></span>
    </div>
    <div class="form-group">
        <label asp-for="Price" class="control-label"></label>
        <input asp-for="Price" class="form-control" />
        <span asp-validation-for="Price" class="text-danger"></span>
    </div>
    <div class="form-group">
        <input type="submit" value="Save" class="btn btn-primary" />
    </div>
</form>

<div>
    <a asp-action="Index">Back to List</a>
</div>
Delete.cshtml:

html
Copy code
@model BookstoreApp.Web.Models.BookViewModel

<h1>Delete Book</h1>

<h3>Are you sure you want to delete this?</h3>
<div>
    <h4>Book</h4>
    <hr />
    <dl class="row">
        <dt class="col-sm-2">Title</dt>
        <dd class="col-sm-10">@Model.Title</dd>

        <dt class="col-sm-2">Author</dt>
        <dd class="col-sm-10">@Model.Author</dd>

        <dt class="col-sm-2">Price</dt>
        <dd class="col-sm-10">@Model.Price</dd>
    </dl>
</div>
<form asp-action="DeleteConfirmed">
    <input type="hidden" asp-for="Id" />
    <input type="submit" value="Delete" class="btn btn-danger" /> |
    <a asp-action="Index">Back to List</a>
</form>
4. Summary
Now, your BookstoreApp.Web project is set up as an MVC application that interacts with the BookstoreApp.WebUI API. Here's a recap of what we have done:

Created an MVC Project (BookstoreApp.Web): This project serves as the frontend for the bookstore application, interacting with the API to display and manage book data.
Configured HttpClient: Set up HttpClient in Program.cs to communicate with the WebAPI.
Created Controllers: Implemented the BooksController to handle CRUD operations by consuming the API.
Created Views: Developed Razor views to display the list of books, book details, and forms for creating, editing, and deleting books.
5. Running the Application
Start the API: Run the BookstoreApp.WebUI project to make the API available.
Start the MVC Application: Run the BookstoreApp.Web project, which should display the list of books and allow you to perform CRUD operations.
With this setup, your bookstore application now has a fully functional MVC frontend that communicates with the WebAPI, completing the Clean Architecture with separate layers for UI, Application, Domain, and Infrastructure.
