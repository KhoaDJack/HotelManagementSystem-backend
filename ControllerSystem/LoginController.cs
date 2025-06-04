using HotelDBFinal.LoginModels;
using Microsoft.AspNetCore.Mvc;

namespace HotelDBFinal.ControllerSystem
{
    public class LoginController : Controller
    {
        // Show login form (GET /Login)
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Handle login form POST /Login
        [HttpPost]
        public IActionResult Index(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // your login validation here
                if (model.Username == "admin" && model.Password == "password123")
                {
                    // After successful login, redirect to swagger UI
                    return Redirect("/swagger");
                }
                ModelState.AddModelError("", "Invalid username or password");
            }
            return View(model);
        }
    }
}