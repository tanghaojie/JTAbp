using Microsoft.AspNetCore.Mvc;

namespace JT.Web.Controllers
{
    public class HomeController : JTControllerBase
    {
        public IActionResult Index()
        {
            return Redirect("/swagger");
        }
    }
}