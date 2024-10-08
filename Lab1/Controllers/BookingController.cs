﻿using Lab1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Lab1.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly HttpClient _client;
        private string baseUrl = "https://localhost:7234";
        public BookingController(HttpClient client)
        {
            _client = client;
        }
        public  async Task<IActionResult> Index()
        {
            ViewData["Title"] = "All Bookings";

            var token = HttpContext.Request.Cookies["jwtToken"];
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"{baseUrl}/api/Booking/getAllBookings");

            var json = await response.Content.ReadAsStringAsync();

            var bookingList = JsonConvert.DeserializeObject<List<Booking>>(json);
        
            return View(bookingList);
        }

        [HttpGet]
        public async Task<IActionResult> CustomerBooking(int Id, string firstName, string lastName)
        {
            ViewData["Title"] = "Customer Bookings";

            ViewBag.FirstName = firstName;
            ViewBag.LastName = lastName;
            ViewBag.CustomerId = Id;

            var token = HttpContext.Request.Cookies["jwtToken"];
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

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

            var token = HttpContext.Request.Cookies["jwtToken"];
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

           
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

            var token = HttpContext.Request.Cookies["jwtToken"];
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PostAsync($"{baseUrl}/api/Booking/addBooking/{customerId}", content);

            var json2 = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<ServiceResult>(json2);

            if (result.Success == true)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index", "Customer");
            }

			else if (result.Success == false)
			{
				TempData["ErrorMessage"] = result.Message;
				return RedirectToAction("Index", "Customer");
			}

			return View(booking);

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int Id, string returnUrl)
        {
            var token = HttpContext.Request.Cookies["jwtToken"];
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"{baseUrl}/api/Booking/getBookingById/{Id}");

            var json = await response.Content.ReadAsStringAsync();

            var booking = JsonConvert.DeserializeObject<Booking>(json);

            ViewData["ReturnUrl"] = returnUrl ?? Url.Action("ViewAll");

            return View(booking);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Booking booking, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(booking);
            }

			var json = JsonConvert.SerializeObject(booking);
			Console.WriteLine("Sending JSON to API: " + json);

			var content = new StringContent(json, Encoding.UTF8, "application/json");

            var token = HttpContext.Request.Cookies["jwtToken"];
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PutAsync($"{baseUrl}/api/Booking/updateBookingById/{booking.Id}", content);

           

            if (!response.IsSuccessStatusCode)
			{
                var errorMessage = await response.Content.ReadAsStringAsync();
				ModelState.AddModelError("", errorMessage);
				return View(booking); // Return to the edit view with the model to show error messages
			}

            else if  (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
                    
            return RedirectToAction("Index");
		}

        [HttpPost]
        public async Task<IActionResult> Delete(int id, string returnUrl = null)
        {

            var token = HttpContext.Request.Cookies["jwtToken"];
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _client.DeleteAsync($"{baseUrl}/api/Booking/deleteBookingById/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Delete Success";
            }

            else if (!response.IsSuccessStatusCode)
            {
                // Handle the failure case
                return BadRequest("Failed to delete booking.");
            }

            else if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(returnUrl);
        }

        //[HttpPost]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var response = await _client.DeleteAsync($"{baseUrl}/api/Booking/deleteBookingById/{id}");

        //    return RedirectToAction("Index", "Customer");
        //}
    }
}
