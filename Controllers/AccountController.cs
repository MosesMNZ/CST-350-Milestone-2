using CST_350_Milestone.Models;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;

namespace CST_350_Milestone.Controllers
{
    public class AccountController : Controller
    {
        private readonly string _connStr;

        public AccountController(IConfiguration config)
        {
            _connStr = config.GetConnectionString("DefaultConnection");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                UserModel user = new UserModel
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Sex = model.Sex,
                    Age = model.Age,
                    State = model.State,
                    Email = model.Email,
                    Username = model.Username,
                };
                user.SetPassword(model.Password);

                UserDAO dao = new UserDAO(_connStr);
                dao.AddUser(user);

                return RedirectToAction("RegistrationSuccess");
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
                UserDAO dao = new UserDAO(_connStr);

                UserModel user = dao.GetUser(model.Username, model.Password);

                if (user != null)
                {
                    HttpContext.Session.SetString("User", user.Username);
                    HttpContext.Session.SetString("FirstName", user.FirstName);

                    return RedirectToAction("LoginSuccess");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
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

        public IActionResult Profile()
        {
            var username = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login");

            UserDAO dao = new UserDAO(_connStr);
            UserModel user = dao.GetUserByUsername(username);

            var model = new ProfileModel
            {
                FirstName = user.FirstName,
                LastName  = user.LastName,
                Sex       = user.Sex,
                Age       = user.Age,
                State     = user.State,
                Email     = user.Email,
                Username  = user.Username,
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Profile(ProfileModel model)
        {
            var username = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login");

            if (ModelState.IsValid)
            {
                try
                {
                    UserDAO dao = new UserDAO(_connStr);
                    UserModel user = dao.GetUserByUsername(username);

                    user.FirstName = model.FirstName;
                    user.LastName  = model.LastName;
                    user.Sex       = model.Sex;
                    user.Age       = model.Age;
                    user.State     = model.State;
                    user.Email     = model.Email;
                    user.Username  = model.Username;

                    dao.UpdateUser(user);

                    // Keep session in sync
                    HttpContext.Session.SetString("User",      model.Username);
                    HttpContext.Session.SetString("FirstName", model.FirstName);

                    TempData["UpdateSuccess"] = true;
                    return RedirectToAction("Profile");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Update failed: " + ex.Message);
                }
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
