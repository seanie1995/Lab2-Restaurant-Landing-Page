using Lab1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Lab1.Controllers
{
    [Authorize]
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

            var token = HttpContext.Request.Cookies["jwtToken"];
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"{baseUrl}/api/Customer/getallcustomers");

            var json = await response.Content.ReadAsStringAsync();

            var customerList = JsonConvert.DeserializeObject<List<Customer>>(json);

           

            return View(customerList);
        }

        public IActionResult Create()
        {
            ViewData["Title"] = "New Customer";

            var token = HttpContext.Request.Cookies["jwtToken"];
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            var token = HttpContext.Request.Cookies["jwtToken"];
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var json = JsonConvert.SerializeObject(customer);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{baseUrl}/api/Customer/addCustomer", content);

           

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            var token = HttpContext.Request.Cookies["jwtToken"];
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"{baseUrl}/api/Customer/getCustomerById/{Id}");

            var json = await response.Content.ReadAsStringAsync();
          
            var customer = JsonConvert.DeserializeObject<Customer>(json);

           

            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Customer customer)
        {
            if (!ModelState.IsValid)
            {
               
                return View(customer);
            }

            var json = JsonConvert.SerializeObject(customer);
            Console.WriteLine("Sending JSON to API: " + json);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var token = HttpContext.Request.Cookies["jwtToken"];
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PutAsync($"{baseUrl}/api/Customer/updateCustomerInfo/{customer.Id}", content);

            

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to update customer. Please try again.");
                return View(customer); // Return to the edit view with the model to show error messages
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.Request.Cookies["jwtToken"];
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.DeleteAsync($"{baseUrl}/api/Customer/deleteCustomer/{id}");

            return RedirectToAction("Index");
        }
    }
}
