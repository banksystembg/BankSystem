namespace BankSystem.Web.Infrastructure.Middleware
{
    public static class MiddlewareConstants
    {
        public static class ContentTypeOptions
        {
            public const string Header = "X-Content-Type-Options";
            public const string NoSniff = "nosniff";
        }

        public static class FeaturePolicy
        {
            public const string Header = "Feature-Policy";
            public const string Ignore = "microphone 'none'; geolocation 'none'; camera 'none'";
        }

        public static class ReferrerPolicy
        {
            public const string Header = "Referrer-Policy";
            public const string NoReferrer = "no-referrer";
        }

        public static class FrameOptions
        {
            public const string Header = "X-Frame-Options";
            public const string Deny = "DENY";
            public const string SameOrigin = "SAMEORIGIN";
            public const string AllowFromUri = "ALLOW-FROM {0}";
        }

        public static class XssProtection
        {
            public const string Header = "X-XSS-Protection";
            public const string Enabled = "1";
            public const string Disabled = "0";
            public const string Block = "1; mode=block";
            public const string Report = "1; report={0}";
        }
    }
}