using Domain.Common;

namespace Domain.Entites;

public partial class Pricing : IBaseEntity<Guid>
{
    public Guid Id { get; set; }

    public Guid ServiceModelId { get; set; }

    public int TokenPerUnit { get; set; }

    public decimal UnitCost { get; set; }

    public virtual ServiceModel ServiceModel { get; set; } = null!;
}
