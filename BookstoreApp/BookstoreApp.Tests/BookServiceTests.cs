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
