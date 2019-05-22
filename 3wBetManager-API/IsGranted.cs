using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using Manager;
using Models;

namespace _3wBetManager_API
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class IsGranted : AuthorizeAttribute
    {
        private readonly string _role;

        public IsGranted(string role = null)
        {
            _role = role ?? "ALL";
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var token = SingletonManager.Instance.TokenManager.GetTokenFromRequest(actionContext.Request);
            if (token == null)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                return;
            }

            var user = SingletonManager.Instance.TokenManager.ValidateToken(token);
            if (user == null)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                return;
            }

            if (_role == User.AdminRole && user["role"] != User.AdminRole)
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
        }
    }
}