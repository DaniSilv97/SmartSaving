using SmartSaving.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SmartSaving.Controllers {
    public class GenericBaseController : Controller {

        protected User? _auth;

        public override void OnActionExecuting(ActionExecutingContext context) {
            base.OnActionExecuting(context);

            UserHelper userHelper = new UserHelper();

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AccessUser"))) {
                HttpContext.Session.SetString("AccessUser", userHelper.serializeUser(userHelper.setGuest()));
            }

            // Having or not a session, here we have a valid user
            _auth = userHelper.deserializeUser(HttpContext.Session.GetString("AccessUser") ?? string.Empty);

            // Set ViewBag properties for the layout
            ViewBag.User = _auth;
            ViewBag.IsAdmin = HasRole(Models.User.Roles.Admin);
            ViewBag.IsAuthenticated = IsAuthenticated();
            ViewBag.AccessLevel = GetAccessLevel();
        }

        protected bool HasRole(User.Roles minimumRole) {
            return _auth != null && _auth.Role >= minimumRole;
        }

        protected bool IsAuthenticated() {
            return _auth != null && _auth.Email != "guest@smart.saving.pt";
        }

        protected int GetAccessLevel() {
            return _auth != null ? (int)_auth.Role : 0;
        }

        protected IActionResult RedirectToLogin() {
            return RedirectToAction("Login", "Auth");
        }

        protected IActionResult RedirectToAccessDenied() {
            return RedirectToAction("AccessDenied", "Auth");
        }
    }
}