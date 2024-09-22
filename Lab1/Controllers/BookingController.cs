using Lab1.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

        public async Task<IActionResult> CustomerBooking(int id)
        {
            ViewData["Title"] = "Customer Bookings";

            var response = await _client.GetAsync($"{baseUrl}/api/Booking/getCustomerBookingsByCustomerId/{id}");

            var json = await response.Content.ReadAsStringAsync();

            var bookingList = JsonConvert.DeserializeObject<List<Booking>>(json);

            return View(bookingList);
        }
    }
}
