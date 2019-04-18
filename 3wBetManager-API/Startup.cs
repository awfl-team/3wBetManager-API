using System.Web.Http;
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
            config.Routes.MapHttpRoute(
                "3wBetManager-API",
                "{controller}/{action}/{id}",
                new {id = RouteParameter.Optional}
            );
            appBuilder.UseCors(CorsOptions.AllowAll);
            appBuilder.UseWebApi(config);
            appBuilder.MapSignalR();
        }
    }
}