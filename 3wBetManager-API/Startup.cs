using System.Web.Http;
using Hub;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Owin;
using Swashbuckle.Application;

namespace _3wBetManager_API
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            var config = new HttpConfiguration();
            config.EnableSwagger(c => { c.SingleApiVersion("v1", "3wBetManager-API"); }).EnableSwaggerUi();
            config.MapHttpAttributeRoutes();
            appBuilder.UseCors(CorsOptions.AllowAll);
            appBuilder.UseWebApi(config);
            GlobalHost.HubPipeline.AddModule(new ErrorHandlingPipelineModule());
            appBuilder.MapSignalR("", new HubConfiguration()
            {
                EnableJSONP = true,
                EnableJavaScriptProxies = true
            });
        }
    }
}