namespace BankSystem.Services.Card
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using BankSystem.Models;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Models.Card;

    public class CardService : BaseService, ICardService
    {
        private readonly ICardHelper cardHelper;
        private readonly IMapper mapper;

        public CardService(BankSystemDbContext context, ICardHelper cardHelper, IMapper mapper)
            : base(context)
        {
            this.cardHelper = cardHelper;
            this.mapper = mapper;
        }

        public async Task<bool> CreateAsync(CardCreateServiceModel model)
        {
            if (!this.IsEntityStateValid(model) ||
                !this.Context.Users.Any(u => u.Id == model.UserId))
            {
                return false;
            }

            string generatedNumber;
            string generated3DigitSecurityCode;
            do
            {
                generatedNumber = this.cardHelper.Generate16DigitNumber();
                generated3DigitSecurityCode = this.cardHelper.Generate3DigitSecurityCode();
            } while (await this.Context.Cards.AnyAsync(a
                => a.Number == generatedNumber && a.SecurityCode == generated3DigitSecurityCode));

            var dbModel = this.mapper.Map<Card>(model);
            dbModel.Number = generatedNumber;
            dbModel.SecurityCode = generated3DigitSecurityCode;

            await this.Context.Cards.AddAsync(dbModel);
            await this.Context.SaveChangesAsync();

            return true;
        }

        public async Task<T> GetAsync<T>(
            string cardNumber,
            string cardExpiryDate,
            string cardSecurityCode,
            string cardOwner)
            where T : CardBaseServiceModel
            => await this.Context
                .Cards
                .AsNoTracking()
                .Where(c =>
                    c.Name == cardOwner &&
                    c.Number == cardNumber &&
                    c.SecurityCode == cardSecurityCode &&
                    c.ExpiryDate == cardExpiryDate)
                .ProjectTo<T>(this.mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

        public async Task<T> GetAsync<T>(string id)
            where T : CardBaseServiceModel
            => await this.Context
                .Cards
                .AsNoTracking()
                .Where(c => c.Id == id)
                .ProjectTo<T>(this.mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

        public async Task<int> GetCountOfAllCardsOwnedByUserAsync(string userId)
            => await this.Context
                .Cards
                .CountAsync(c => c.UserId == userId);

        public async Task<IEnumerable<T>> GetCardsAsync<T>(string userId, int pageIndex = 1, int count = int.MaxValue)
            where T : CardBaseServiceModel
            => await this.Context
                .Cards
                .AsNoTracking()
                .Where(c => c.UserId == userId)
                .Skip((pageIndex - 1) * count)
                .Take(count)
                .ProjectTo<T>(this.mapper.ConfigurationProvider)
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