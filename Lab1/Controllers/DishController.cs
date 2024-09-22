using Lab1.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Lab1.Controllers
{
    public class DishController : Controller
    {
        string url = "https://localhost:7234";

        private readonly HttpClient _client;
        private string baseUrl = "https://localhost:7234";
        public DishController(HttpClient client)
        {
            _client = client;
        }

        public async Task<IActionResult> Index()
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

        [HttpPost]
        public async Task<IActionResult> Create(Dish dish)
        {
            if (!ModelState.IsValid)
            {
                return View(dish);
            }

            var json = JsonConvert.SerializeObject(dish);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{baseUrl}/api/Dishes/addNewDish", content);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var response = await _client.GetAsync($"{baseUrl}/api/Dishes/getDishById/{id}");

            var json = await response.Content.ReadAsStringAsync();

            var dish = JsonConvert.DeserializeObject<Dish>(json);

            return View(dish);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Dish dish)
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

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _client.DeleteAsync($"{baseUrl}/api/Dishes/deleteDishById/{id}");

            return RedirectToAction("Index");
        }
    }
}

