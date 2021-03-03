namespace BankSystem.Services
{
    using Common.Utils;
    using Data;

    public abstract class BaseService
    {
        protected readonly BankSystemDbContext Context;

        protected BaseService(BankSystemDbContext context)
            => this.Context = context;

        protected bool IsEntityStateValid(object model)
            => ValidationUtil.IsObjectValid(model);
    }
}