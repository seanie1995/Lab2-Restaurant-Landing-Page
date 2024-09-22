using Microsoft.AspNetCore.Mvc;

namespace Lab1.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult AdminMenu()
        {
            return View();
        }
    }
}
