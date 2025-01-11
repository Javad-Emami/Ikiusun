using Domain.Common;

namespace Domain.Entites;

public class CurrencyExchangeRate : IBaseEntity
{
    public int Id { get; set; }

    public decimal CurrencyExchangeRate1 { get; set; }

    public virtual ICollection<Pricing> Pricings { get; set; } = new List<Pricing>();

}
