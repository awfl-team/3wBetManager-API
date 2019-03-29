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
            var config = new HttpConfiguration();
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
            config.EnableSwagger(c => { c.SingleApiVersion("v1", "3wBetManager-API"); }).EnableSwaggerUi();
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                "3wBetManager-API",
                "{controller}/{action}/{id}",
                new {id = RouteParameter.Optional}
            );
            appBuilder.UseWebApi(config);
        }
    }
}