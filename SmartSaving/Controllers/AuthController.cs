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
        public IActionResult Profile(User updatedUser, string newPassword, string confirmPassword) {
            if (!IsAuthenticated()) {
                return RedirectToLogin();
            }

            if (ModelState.IsValid) {
                UserHelper userHelper = new UserHelper();

                // Update basic info
                updatedUser.Role = _auth.Role; // Keep existing role

                // Handle password change
                if (!string.IsNullOrWhiteSpace(newPassword)) {
                    if (newPassword != confirmPassword) {
                        ViewBag.Error = "New passwords do not match.";
                        return View(_auth);
                    }
                    updatedUser.Password = newPassword;
                }

                try {
                    if (userHelper.update(updatedUser, _auth.GuidUser)) {
                        // Update session with new user data
                        User? refreshedUser = userHelper.get(_auth.GuidUser);
                        if (refreshedUser != null) {
                            refreshedUser.Password = ""; // Clear password for security
                            HttpContext.Session.SetString("AccessUser", userHelper.serializeUser(refreshedUser));
                        }

                        ViewBag.Success = "Profile updated successfully.";
                        return View(refreshedUser);
                    }
                    else {
                        ViewBag.Error = "An error occurred while updating your profile.";
                        return View(_auth);
                    }
                }
                catch (Exception ex) {
                    ViewBag.Error = ex.Message;
                    return View(_auth);
                }
            }

            return View(_auth);
        }
    }
}