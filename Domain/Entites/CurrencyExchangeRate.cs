namespace Domain.Entites;

public class CurrencyExchangeRate
{
    public int Id { get; set; }

    public decimal CurrencyExchangeRate1 { get; set; }

    public virtual ICollection<Pricing> Pricings { get; set; } = new List<Pricing>();

}
