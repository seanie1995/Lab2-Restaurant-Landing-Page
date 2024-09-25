using Lab1.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Lab1.Controllers
{
	public class TableController : Controller
	{
        private readonly HttpClient _client;
        private string baseUrl = "https://localhost:7234";
        public TableController(HttpClient client)
        {
            _client = client;
        }
		[HttpGet]
        public async Task <IActionResult> Index()
		{
			ViewData["Title"] = "Tables Overview";

			var response = await _client.GetAsync($"{baseUrl}/api/Table/getAllTables");

			var json = await response.Content.ReadAsStringAsync();

			var tablesList = JsonConvert.DeserializeObject<List<Table>>(json);	

			return View(tablesList);
		}

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Title"] = "New Table";

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(int capacity)
        {
            ViewData["Title"] = "Add New Table";

            // Send the capacity as part of the URL, no content needed
            var response = await _client.PostAsync($"{baseUrl}/api/Table/addTable/{capacity}", null);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _client.GetAsync($"{baseUrl}/api/Table/getTableById/{id}");

            var json = await response.Content.ReadAsStringAsync();

            var table = JsonConvert.DeserializeObject<Table>(json);

            return View(table);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Table table)
        {      
            int tableId = table.Id; 
            int newCapacity = table.Capacity;

            // Construct the URL using tableId and the newCapacity
            var response = await _client.PutAsync($"{baseUrl}/api/Table/updateTable/{tableId}/{newCapacity}", null);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index"); // Redirect after successful update
            }
            else
            {
                // Handle error scenario
                ModelState.AddModelError("", "Failed to update the table capacity");
                return View(table); // Return the view with the current model and errors
            }
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {        
            var response = await _client.DeleteAsync($"{baseUrl}/api/Table/deleteTableById/{id}");

            return RedirectToAction("Index");
        }

    }
}
