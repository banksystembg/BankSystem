namespace BankSystem.Web.Infrastructure.Middleware
{
    using System.Collections.Generic;

    public class SecurityHeadersPolicy
    {
        public IDictionary<string, string> HeadersToSet { get; } = new Dictionary<string, string>();

        public ISet<string> HeadersToRemove { get; } = new HashSet<string>();
    }
}