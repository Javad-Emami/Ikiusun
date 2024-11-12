using Domain.Common;

namespace Domain.Entites;

public class Deposite : IBaseEntity<Guid>
{
    public Guid Id { get; set; }

    public Guid WalletId { get; set; }

    public decimal DepositeAmount { get; set; }

    public DateTime DepositeDate { get; set; }

    public virtual Wallet Wallet { get; set; } = null!;
}
