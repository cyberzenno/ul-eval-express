using Microsoft.AspNetCore.Mvc;
//using ul.eval.express.web.Models;

namespace ul.eval.express.web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Resources()
        {
            return View();
        }

    }
}
