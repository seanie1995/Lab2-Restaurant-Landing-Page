using Lab1.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Lab1.Controllers
{
    public class CustomerController : Controller
    {
        private readonly HttpClient _client;
        private string baseUrl = "https://localhost:7234";
        public CustomerController(HttpClient client)
        {
            _client = client;
        }
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Customers Overview";

            var response = await _client.GetAsync($"{baseUrl}/api/Customer/getallcustomers");

            var json = await response.Content.ReadAsStringAsync();

            var customerList = JsonConvert.DeserializeObject<List<Customer>>(json);

            return View(customerList);
        }

        public IActionResult Create()
        {
            ViewData["Title"] = "New Customer";

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            var json = JsonConvert.SerializeObject(customer);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{baseUrl}/api/Customer/addCustomer", content);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _client.DeleteAsync($"{baseUrl}/api/Customer/deleteCustomer/{id}");

            return RedirectToAction("Index");
        }
    }
}
