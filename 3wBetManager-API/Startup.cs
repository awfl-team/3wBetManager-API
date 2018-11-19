using System.Web.Http;
using Owin;
using Swashbuckle.Application;

namespace _3wBetManager_API
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.EnableSwagger(c =>
            {
                c.SingleApiVersion("v1", "3wBetManager-API");
            }).EnableSwaggerUi();
            config.Routes.MapHttpRoute(
                name: "3wBetManager-API",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);
        }
    }
}
