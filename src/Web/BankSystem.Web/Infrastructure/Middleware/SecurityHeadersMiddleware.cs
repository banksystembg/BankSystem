namespace BankSystem.Web.Infrastructure.Middleware
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate next;
        private readonly SecurityHeadersPolicy policy;

        public SecurityHeadersMiddleware(RequestDelegate next, SecurityHeadersPolicy policy)
        {
            this.next = next;
            this.policy = policy;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var headers = context.Response.Headers;

            foreach (var (key, value) in this.policy.HeadersToSet)
            {
                headers[key] = value;
            }

            foreach (var header in this.policy.HeadersToRemove)
            {
                headers.Remove(header);
            }

            await this.next(context);
        }
    }
}