using Lab1.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Lab1.Controllers
{
    public class AdminController : Controller
    {
        private readonly HttpClient _client;
        private string baseUrl = "https://localhost:7234";

        public AdminController(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri("https://localhost:7234");
        }

        public IActionResult AdminMenu()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel admin)
        {
            var response = await _client.PostAsJsonAsync("api/Admin/login", admin);

            if (!response.IsSuccessStatusCode)
            {
                return View(admin);
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<TokenResponse>(jsonResponse);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token.Token);

            var claims = jwtToken.Claims.ToList();

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrinicipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrinicipal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = jwtToken.ValidTo
            });

            HttpContext.Response.Cookies.Append("jwtToken", token.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = jwtToken.ValidTo
            });

            return RedirectToAction("Index", "Home");   
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Response.Cookies.Delete("jwtToken");

            return RedirectToAction("Index", "Home");
        }
    }
}
 