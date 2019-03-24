namespace BankSystem.Services.Implementations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using BankSystem.Models;
    using Data;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Models.Card;

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

            string generatedNumber;
            string generated3DigitSecurityCode;
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

        public async Task<T> GetAsync<T>(
            string cardNumber,
            string cardExpiryDate,
            string cardSecurityCode,
            string cardOwner)
            where T : CardBaseServiceModel
            => await this.Context
                .Cards
                .Where(c =>
                    c.Name == cardOwner &&
                    c.Number == cardNumber &&
                    c.SecurityCode == cardSecurityCode &&
                    c.ExpiryDate == cardExpiryDate)
                .ProjectTo<T>()
                .SingleOrDefaultAsync();

        public async Task<T> GetAsync<T>(string id)
            where T : CardBaseServiceModel
            => await this.Context.Cards
                .Where(c => c.Id == id)
                .ProjectTo<T>()
                .SingleOrDefaultAsync();

        public async Task<IEnumerable<T>> GetAllCardsAsync<T>(string userId)
            where T : CardBaseServiceModel
            => await this.Context
                .Cards
                .Where(c => c.UserId == userId)
                .ProjectTo<T>()
                .ToArrayAsync();

        public async Task<bool> DeleteAsync(string id)
        {
            if (id == null)
            {
                return false;
            }

            var card = await this.Context
                .Cards
                .Where(c => c.Id == id)
                .SingleOrDefaultAsync();

            if (card == null)
            {
                return false;
            }

            this.Context.Cards.Remove(card);
            await this.Context.SaveChangesAsync();

            return true;
        }
    }
}
