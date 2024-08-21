using BookstoreApp.Application.Interfaces;
using BookstoreApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookstoreApp.Infrastructure;

namespace BookstoreApp.Application.Services
{
   
    public class BookServiceDB : IBookService
    {
        private readonly IBookRepository _repository;
        public BookServiceDB(IBookRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Book>> GetBooksAsync()
        {
            return await _repository.GetBooksAsync();
        }

        public async Task<Book> GetBookByIdAsync(int id)
        {
            var book = await _repository.GetBookByIdAsync(id);
            return book;
        }

        public async Task AddBookAsync(Book book)
        {
            await _repository.AddBookAsync(book);
        }

        public async Task UpdateBookAsync(Book book)
        {
            await _repository.UpdateBookAsync(book);
        }

        public async Task DeleteBookAsync(int id)
        {
           await _repository.DeleteBookAsync(id);
        }
    }
}
