using Application.Interfaces;
using AutoMapper;
using Domain.Entites;
using Persistance;
using Service.Repository;

namespace Service.Services;

public class PricingService : BaseService<Pricing, Guid>, IPricingService
{
    public PricingService(SqlDbContext db, IMapper mapper) : base(db, mapper)
    {
    }
}
