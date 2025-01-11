using Application.Interfaces;
using AutoMapper;
using Domain.Entites;
using Persistance;
using Service.Repository;

namespace Service.Services;

public class CurrencyExchangeRateService : BaseService<CurrencyExchangeRate>, ICurrencyExchangeRateService
{
    public CurrencyExchangeRateService(SqlDbContext context, IMapper mapper) : base(context, mapper)
    {
    }
}
