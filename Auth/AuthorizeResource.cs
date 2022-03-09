using System;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace acme_order.Auth
{
    public sealed class AuthorizeResource : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            const string accessTokenKey = "Authorization";
            var headers = context.HttpContext.Request.Headers;

            if (!headers.Keys.Any(x => x.Equals(accessTokenKey))) throw new AuthenticationException();
            var accessToken = headers[accessTokenKey];
            accessToken = accessToken.ToString().Replace("Bearer ","");


            if (!string.IsNullOrEmpty(accessToken)) throw new AuthenticationException();
            VerifyToken(accessToken);
        }


        private static async void VerifyToken(string accessToken)
        {
            var json = JsonConvert.SerializeObject(new TokenRequest(accessToken));
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            const string url = "http://localhost:8083/verify-token";
            using var client = new HttpClient();

            var response = await client.PostAsync(url, data);

            if (!response.IsSuccessStatusCode)
            {
                throw new AuthenticationException();
            }
        }


        private struct TokenRequest
        {
            public TokenRequest(string accessToken)
            {
                _accessToken = accessToken;
            }

            private string _accessToken;
        }
    }
}