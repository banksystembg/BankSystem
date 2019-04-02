namespace BankSystem.Common.AutoMapping.Interfaces
{
    using AutoMapper;

    public interface IHaveCustomMapping
    {
        void ConfigureMapping(Profile mapper);
    }
}