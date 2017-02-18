namespace AzureAuthVanilla.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Web.Configuration;
    using System.Web.Mvc;

    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ClaimsPrincipal principal = User as ClaimsPrincipal;
            var model = principal.Claims.Select(c => new ClaimModel { Name = c.Type, Value = c.Value }).ToList();
            BuildUpGroupsFromGraphAPI(model);
            return View(model);
        }

        private void BuildUpGroupsFromGraphAPI(List<ClaimModel> model)
        {
            string clientId = WebConfigurationManager.AppSettings["b2c:ClientId"];
            string secret = WebConfigurationManager.AppSettings["b2c:ClientSecret"];
            string tenant = WebConfigurationManager.AppSettings["b2c:Tenant"];
            var graphClient = new GraphClient(clientId, secret, tenant);

            string userObjectId = (User as ClaimsPrincipal).FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            List<string> groupsResult = graphClient.MemberOf(userObjectId).Result;
            foreach (string groupId in groupsResult)
            {
                model.Add(new ClaimModel {Name = "GROUP", Value = groupId});
            }
        }
    }
}