namespace BankSystem.Common.Utils
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public static class ValidationUtil
    {
        public static bool IsObjectValid(object model)
        {
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();

            return Validator.TryValidateObject(model, validationContext, validationResults,
                true);
        }
    }
}