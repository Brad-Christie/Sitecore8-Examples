using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Sitecore8.ServicesClient.Pipelines.Loader
{
    public class EnableCorsGlobally
    {
        public void Process(PipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            this.EnableCors(GlobalConfiguration.Configuration);
        }

        private void EnableCors(HttpConfiguration config)
        {
            // Sitecore, by default, enabled CORS (but not on its native
            // API methods). This will allow us to access the /item
            // API from another site (through JavaScript).
            // See docs section 4.4
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));
        }
    }
}
