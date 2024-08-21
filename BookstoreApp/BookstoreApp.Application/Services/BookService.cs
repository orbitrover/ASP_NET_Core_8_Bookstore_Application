using BookstoreApp.Application.Interfaces;
using BookstoreApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            book.Id = _books.Count() + 1;
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
