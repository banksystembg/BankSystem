namespace DemoShop.Services.Implementations
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Data;

    public abstract class BaseService
    {
        protected readonly DemoShopDbContext Context;

        protected BaseService(DemoShopDbContext context)
            => this.Context = context;

        public static bool IsEntityStateValid(object model)
        {
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();

            return Validator.TryValidateObject(model, validationContext, validationResults,
                true);
        }
    }
}