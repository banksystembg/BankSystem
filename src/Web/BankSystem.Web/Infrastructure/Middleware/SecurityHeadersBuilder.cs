namespace BankSystem.Web.Infrastructure.Middleware
{
    using System;

    public class SecurityHeadersBuilder
    {
        private readonly SecurityHeadersPolicy policy = new SecurityHeadersPolicy();

        public SecurityHeadersBuilder AddDefaultSecurePolicy()
        {
            this.AddFrameOptionsSameOrigin();
            this.AddFeature();
            this.AddReferrer();
            this.AddXssProtection();
            this.AddContentTypeOptionsNoSniff();

            return this;
        }

        public void AddFrameOptionsSameOrigin()
            => this.AddCustomHeader(MiddlewareConstants.FrameOptions.Header, MiddlewareConstants.FrameOptions.SameOrigin);

        public void AddFeature()
            => this.AddCustomHeader(MiddlewareConstants.FeaturePolicy.Header, MiddlewareConstants.FeaturePolicy.Ignore);

        public void AddReferrer()
            => this.AddCustomHeader(MiddlewareConstants.ReferrerPolicy.Header, MiddlewareConstants.ReferrerPolicy.NoReferrer);

        public void AddXssProtection()
            => this.AddCustomHeader(MiddlewareConstants.XssProtection.Header, MiddlewareConstants.XssProtection.Block);

        public void AddContentTypeOptionsNoSniff()
            => this.AddCustomHeader(MiddlewareConstants.ContentTypeOptions.Header, MiddlewareConstants.ContentTypeOptions.NoSniff);

        public SecurityHeadersBuilder AddCustomHeader(string header, string value)
        {
            if (string.IsNullOrEmpty(header))
            {
                throw new ArgumentNullException(nameof(header));
            }

            this.policy.HeadersToSet[header] = value;

            return this;
        }

        public SecurityHeadersBuilder RemoveHeader(string header)
        {
            if (string.IsNullOrEmpty(header))
            {
                throw new ArgumentNullException(nameof(header));
            }

            this.policy.HeadersToRemove.Add(header);

            return this;
        }

        public SecurityHeadersPolicy Policy() => this.policy;
    }
}