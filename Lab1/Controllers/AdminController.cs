using Lab1.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Lab1.Controllers
{
    public class AdminController : Controller
    {
        string url = "https://localhost:7234";

        private readonly HttpClient _client;
        private string baseUrl = "https://localhost:7234";
        public AdminController(HttpClient client)
        {
            _client = client;
        }

        public async Task<IActionResult> Dishes()
        {
            ViewData["Title"] = "Dish Overview";

            var response = await _client.GetAsync($"{baseUrl}/api/Dishes/getAllDishes");

            var json = await response.Content.ReadAsStringAsync();

            var dishList = JsonConvert.DeserializeObject<List<Dish>>(json);

            return View(dishList);
        }

        public async Task<IActionResult> AdminMenu()
        {
            return View();
        }

        public async Task<IActionResult> EditDish(int id)
        {
            var response = await _client.GetAsync($"{baseUrl}/api/Dishes/getDishById/{id}");

            var json = await response.Content.ReadAsStringAsync();

            var dish = JsonConvert.DeserializeObject<Dish>(json);

            return View(dish);
        }
        [HttpPost]
        public async Task<IActionResult> EditDish(Dish dish)
        {
            if (!ModelState.IsValid)
            {
                return View(dish);
            } 

            var json = JsonConvert.SerializeObject(dish);
            Console.WriteLine("Sending JSON to API: " + json);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"{baseUrl}/api/Dishes/updateDishById/{dish.Id}", content);
            if (!response.IsSuccessStatusCode)
            {            
                ModelState.AddModelError("", "Failed to update the dish. Please try again.");
                return View(dish); // Return to the edit view with the model to show error messages
            }

            return RedirectToAction("Dishes");
        }
    }
}

