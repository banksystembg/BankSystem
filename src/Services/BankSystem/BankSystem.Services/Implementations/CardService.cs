namespace BankSystem.Services.Implementations
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using BankSystem.Models;
    using Data;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Models.Card;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CardService : BaseService, ICardService
    {
        private readonly ICardHelper cardHelper;

        public CardService(BankSystemDbContext context, ICardHelper cardHelper)
            : base(context)
        {
            this.cardHelper = cardHelper;
        }

        public async Task<string> CreateAsync(CardCreateServiceModel model)
        {
            if (!this.IsEntityStateValid(model) ||
                !this.Context.Users.Any(u => u.Id == model.UserId))
            {
                return null;
            }

            long generatedNumber;
            int generated3DigitSecurityCode;
            do
            {
                generatedNumber = this.cardHelper.Generate16DigitNumber();
                generated3DigitSecurityCode = this.cardHelper.Generate3DigitSecurityCode();
            } while (await this.Context.Cards.AnyAsync(a => a.Number == generatedNumber && a.SecurityCode == generated3DigitSecurityCode));

            var dbModel = Mapper.Map<Card>(model);
            dbModel.Number = generatedNumber;
            dbModel.SecurityCode = generated3DigitSecurityCode;

            await this.Context.Cards.AddAsync(dbModel);
            await this.Context.SaveChangesAsync();

            return dbModel.Id;
        }

        public async Task<IEnumerable<T>> GetAllCardsAsync<T>(string userId)
            where T : CardBaseServiceModel
            => await this.Context
                .Cards
                .Where(c => c.UserId == userId)
                .ProjectTo<T>()
                .ToArrayAsync();
    }
}
