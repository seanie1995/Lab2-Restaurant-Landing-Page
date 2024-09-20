using Lab1.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Lab1.Controllers
{
    public class MenuController : Controller
    {
        string url = "https://localhost:7234";

        private readonly HttpClient _client;
        private string baseUrl = "https://localhost:7234";
        public MenuController(HttpClient client)
        {
            _client = client;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Menu";

            var response = await _client.GetAsync($"{baseUrl}api/Dishes/getAllDishes");

            var json = await response.Content.ReadAsStringAsync();

            var dishList = JsonConvert.DeserializeObject<List<Dish>>(json);

            return View(dishList);
        }
    }
}
