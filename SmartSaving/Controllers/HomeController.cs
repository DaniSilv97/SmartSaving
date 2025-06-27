using Microsoft.AspNetCore.Mvc;

namespace SmartSaving.Controllers {
    public class HomeController : GenericBaseController {
        public IActionResult Index() {
            return View();
        }
    }
}
