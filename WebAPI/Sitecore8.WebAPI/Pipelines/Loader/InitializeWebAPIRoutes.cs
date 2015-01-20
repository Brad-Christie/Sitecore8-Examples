using Sitecore.Configuration;
using Sitecore.Pipelines;
using System.Web.Http;
using System.Web.Routing;

namespace Sitecore8.WebAPI.Pipelines.Loader
{
    //
    // Defines the routes we want exposed for our API. You could go generic,
    // but I decided to expose each one explicitly. Also, as an added bonus,
    // you can change the prefix via a config file change.
    //
    public class InitializeWebAPIRoutes
    {
        public void Process(PipelineArgs args)
        {
            // map our API calls
            this.MapRoutes(args, RouteTable.Routes);

            // In case you want to use AttributeRouting
            //this.MapRoutes(args, GlobalConfiguration.Configuration);
        }

        protected virtual void MapRoutes(PipelineArgs args, RouteCollection routes)
        {
            var routeBase = Settings.GetSetting("Sitecore8.WebAPI.RouteBase", "api/");
            routes.MapHttpRoute(
                "ProductService-Get",
                routeBase + "products/{id}",
                new { controller = "Product", action = "Get" },
                new { id = @"^\d+$" }
            );
            routes.MapHttpRoute(
                "ProductService-GetAll",
                routeBase + "products",
                new { controller = "Product", action = "GetAll" }
            );
        }

        protected virtual void MapRoutes(PipelineArgs args, HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
        }
    }
}