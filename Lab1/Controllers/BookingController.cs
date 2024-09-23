using Lab1.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Lab1.Controllers
{
    public class BookingController : Controller
    {
        private readonly HttpClient _client;
        private string baseUrl = "https://localhost:7234";
        public BookingController(HttpClient client)
        {
            _client = client;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CustomerBooking(int Id, string firstName, string lastName)
        {
            ViewData["Title"] = "Customer Bookings";

            ViewBag.FirstName = firstName;
            ViewBag.LastName = lastName;
            ViewBag.CustomerId = Id;
           
            var response = await _client.GetAsync($"{baseUrl}/api/Booking/getCustomerBookingsByCustomerId/{Id}");

            var json = await response.Content.ReadAsStringAsync();

            var bookingList = JsonConvert.DeserializeObject<List<Booking>>(json);

            return View(bookingList);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int customerId, string firstName)
        {
            ViewBag.CustomerId = customerId;
            ViewBag.FirstName = firstName;  
            ViewData["Title"] = "New Booking";
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Booking booking)
        {
            int customerId = booking.CustomerId;

            if (!ModelState.IsValid)
            {
                return View(booking);
            }

            var json = JsonConvert.SerializeObject(booking);

            Console.WriteLine($"Json being sent: {json}");

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{baseUrl}/api/Booking/addBooking/{customerId}", content);

            return RedirectToAction("Index", "Customer");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _client.DeleteAsync($"{baseUrl}/api/Booking/deleteBookingById/{id}");

            return RedirectToAction("Index", "Customer");
        }
    }
}
