using BookstoreApp.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;

namespace BookstoreApp.WebUI.Controllers
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
