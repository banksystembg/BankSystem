namespace BankSystem.Web.TagHelpers
{
    using Microsoft.AspNetCore.Razor.TagHelpers;
    using Microsoft.Extensions.Configuration;

    [HtmlTargetElement("recaptcha")]
    public class ReCaptchaTagHelper : TagHelper
    {
        private readonly IConfiguration configuration;

        public ReCaptchaTagHelper(IConfiguration configuration)
            => this.configuration = configuration;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var siteKey = this.configuration["ReCaptcha:SiteKey"];

            output.TagName = "div";
            output.Attributes.SetAttribute("class", "g-recaptcha");
            output.Attributes.SetAttribute("data-sitekey", siteKey);

            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}