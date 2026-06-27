using CST_350_Milestone.Models;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;

namespace CST_350_Milestone.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Hash the password using BCrypt
                    model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
                    
                    // TODO: Add database functionality to save user registration
                    // If database save fails, redirect to error page
                    
                    return RedirectToAction("RegistrationSuccess");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Registration failed: " + ex.Message;
                    return RedirectToAction("RegistrationError");
                }
            }

            return View(model);
        }

        public IActionResult RegistrationSuccess()
        {
            return View();
        }

        public IActionResult RegistrationError()
        {
            ViewBag.ErrorMessage = TempData["ErrorMessage"] ?? "An error occurred during registration.";
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add database authentication functionality
                    // Example: Verify password using BCrypt
                    // string storedHash = /* get from database */;
                    // bool isValid = BCrypt.Net.BCrypt.Verify(model.Password, storedHash);
                    
                    // Placeholder validation - replace with actual database check
                    if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
                    {
                        TempData["ErrorMessage"] = "Invalid username or password.";
                        return RedirectToAction("LoginError");
                    }
                    
                    // Set session variable to mark user as authenticated
                    HttpContext.Session.SetString("UserAuthenticated", "true");
                    HttpContext.Session.SetString("Username", model.Username);
                    return RedirectToAction("LoginSuccess");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Login failed: " + ex.Message;
                    return RedirectToAction("LoginError");
                }
            }

            return View(model);
        }

        public IActionResult LoginSuccess()
        {
            return View();
        }

        public IActionResult LoginError()
        {
            ViewBag.ErrorMessage = TempData["ErrorMessage"] ?? "Invalid login credentials.";
            return View();
        }
    }
}
