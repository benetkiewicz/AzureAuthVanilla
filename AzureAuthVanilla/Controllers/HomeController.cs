namespace AzureAuthVanilla.Controllers
{
    using System.Linq;
    using System.Security.Claims;
    using System.Web.Mvc;

    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ClaimsPrincipal principal = User as ClaimsPrincipal;
            var model = principal.Claims.Select(c => new ClaimModel { Name = c.Type, Value = c.Value }).ToList();
            return View(model);
        }
    }

    public class ClaimModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

}