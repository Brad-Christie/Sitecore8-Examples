using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore8.ServicesClient.Controllers;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sitecore8.ServicesClient.Pipelines.Loader
{
    public class InitializeDemoSiteRoutes
    {
        public void Process(PipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            this.RegisterRoutes(args, RouteTable.Routes);
        }

        private void RegisterRoutes(PipelineArgs args, RouteCollection routes)
        {
            routes.MapRoute(
                "Sitecore8-ServicesClient",
                "Sitecore8-ServicesClient/{action}/{id}",
                new { controller = "DemoSite", action = "Index", id = UrlParameter.Optional },
                new[] { typeof(DemoSiteController).Namespace }
            );
        }
    }
}