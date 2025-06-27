using Microsoft.AspNetCore.Mvc;
using SmartSaving.Models;

namespace SmartSaving.Controllers {
    public class AuthController : GenericBaseController {

        [HttpGet]
        public IActionResult Login() {
            // If already authenticated, redirect to home
            if (IsAuthenticated()) {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password) {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) {
                ViewBag.Error = "Email and password are required.";
                return View();
            }

            UserHelper userHelper = new UserHelper();
            User? authenticatedUser = userHelper.authUser(email, password);

            if (authenticatedUser != null && authenticatedUser.Email != "guest@smart.saving.pt") {
                // Authentication successful
                HttpContext.Session.SetString("AccessUser", userHelper.serializeUser(authenticatedUser));
                return RedirectToAction("Index", "Home");
            }
            else {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }
        }

        public IActionResult Logout() {
            // Clear session and set guest user
            UserHelper userHelper = new UserHelper();
            HttpContext.Session.SetString("AccessUser", userHelper.serializeUser(userHelper.setGuest()));

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register() {
            // If already authenticated, redirect to home
            if (IsAuthenticated()) {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Register(User user, string confirmPassword) {
            if (ModelState.IsValid) {
                if (user.Password != confirmPassword) {
                    ViewBag.Error = "Passwords do not match.";
                    return View(user);
                }

                UserHelper userHelper = new UserHelper();

                // Check if email already exists
                User? existingUser = userHelper.getUserByEmail(user.Email);
                if (existingUser != null) {
                    ViewBag.Error = "An account with this email already exists.";
                    return View(user);
                }

                try {
                    // Set default role for new users
                    user.Role = Models.User.Roles.User;

                    if (userHelper.create(user)) {
                        ViewBag.Success = "Account created successfully. You can now log in.";
                        return View("Login");
                    }
                    else {
                        ViewBag.Error = "An error occurred while creating your account.";
                        return View(user);
                    }
                }
                catch (Exception ex) {
                    ViewBag.Error = ex.Message;
                    return View(user);
                }
            }

            return View(user);
        }

        public IActionResult AccessDenied() {
            return View();
        }

        [HttpGet]
        public IActionResult Profile() {
            if (!IsAuthenticated()) {
                return RedirectToLogin();
            }
            TrackerHelper trackerHelper = new TrackerHelper();
            ViewBag.TrackerCount = trackerHelper.getTrackerCountByUser(_auth.GuidUser.ToString());
            return View(_auth);
        }

        [HttpPost]
        public IActionResult Profile(IFormCollection form, string newPassword, string confirmPassword) {
            if (!IsAuthenticated()) {
                return RedirectToLogin();
            }

            UserHelper userHelper = new UserHelper();
            User existingUser = userHelper.get(_auth.GuidUser);

            if (existingUser == null) {
                ViewBag.Error = "User not found.";
                return View(_auth);
            }

            try {
                // Apply updates safely with validation
                string name = form["Name"];
                string email = form["Email"];

                existingUser.Name = name;
                existingUser.Email = email;

                // Password change, if applicable
                if (!string.IsNullOrWhiteSpace(newPassword)) {
                    if (newPassword != confirmPassword) {
                        ViewBag.Error = "New passwords do not match.";
                        return View(existingUser);
                    }

                    existingUser.Password = newPassword;
                }

                // Preserve role
                existingUser.Role = _auth.Role;

                if (userHelper.update(existingUser, _auth.GuidUser)) {
                    User refreshedUser = userHelper.get(_auth.GuidUser);
                    if (refreshedUser != null) {
                        refreshedUser.Password = "";
                        HttpContext.Session.SetString("AccessUser", userHelper.serializeUser(refreshedUser));
                    }

                    ViewBag.Success = "Profile updated successfully.";
                    return View(refreshedUser);
                }
                else {
                    ViewBag.Error = "Failed to update profile.";
                    return View(existingUser);
                }
            }
            catch (ArgumentException ex) {
                ViewBag.Error = ex.Message;
                return View(_auth);
            }
            catch (Exception ex) {
                ViewBag.Error = "Unexpected error: " + ex.Message;
                return View(_auth);
            }
        }
    }
}