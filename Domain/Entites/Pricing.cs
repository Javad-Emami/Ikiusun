using Domain.Common;

namespace Domain.Entites;

public class Pricing : IBaseEntity<Guid>
{
    public Guid Id { get; set; }

    public int ServiceModelId { get; set; }

    public int TokenPerUnit { get; set; }

    public decimal UnitCost { get; set; }

    public int CurrencyExchangeRateId { get; set; }

    public virtual CurrencyExchangeRate CurrencyExchangeRate { get; set; } = null!;

    public virtual ServiceModel ServiceModel { get; set; } = null!;
}
