using Domain.Common;

namespace Domain.Entites;

public class Pricing : IBaseEntity<Guid>
{
    public Guid Id { get; set; }

    public int ServiceModelId { get; set; }

    public decimal? InputTokenCost { get; set; }

    public decimal? OutputTokenCost { get; set; }

    public int? ImageQualityId { get; set; }

    public int? ImageResolutionId { get; set; }

    public int? AudioModelId { get; set; }

    public decimal? ImageAudioPrice { get; set; }

    public int CurrencyExchangeRateId { get; set; }

    public virtual CurrencyExchangeRate CurrencyExchangeRate { get; set; } = null!;

    //public virtual ServiceModel ServiceModel { get; set; } = null!;

    public virtual ICollection<UserRequest> UserRequests { get; set; } = new List<UserRequest>();
}
