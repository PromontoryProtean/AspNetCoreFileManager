using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreFileManager.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            //return RedirectToAction("", "FileManager");
            return Redirect("file-manager");
        }
    }
}
