namespace AzureAuthVanilla.Controllers
{
    using System.Web.Mvc;

    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return Content(string.Format("Hello {0}", User.Identity.Name));
        }
    }
}