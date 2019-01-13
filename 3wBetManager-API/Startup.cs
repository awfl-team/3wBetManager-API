using System.Web.Http;
using System.Web.Http.Cors;
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
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
            config.EnableSwagger(c =>
            {
                c.SingleApiVersion("v1", "3wBetManager-API");
            }).EnableSwaggerUi();
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "3wBetManager-API",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            appBuilder.UseWebApi(config);
        }
    }

}
