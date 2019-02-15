namespace BankSystem.Services.Implementations
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Data;

    public abstract class BaseService
    {
        protected readonly BankSystemDbContext Context;

        protected BaseService(BankSystemDbContext context)
        {
            this.Context = context;
        }

        protected bool IsEntityStateValid(object model)
        {
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();

            return Validator.TryValidateObject(model, validationContext, validationResults,
                validateAllProperties: true);
        }
    }
}
