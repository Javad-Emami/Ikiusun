using Domain.Common;

namespace Domain.Entites;

public partial class Wallet : IBaseEntity<Guid>
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public decimal BalanceAmount { get; set; }

    public DateTime CreationDate { get; set; }

    public virtual ICollection<Deposite> Deposites { get; set; } = new List<Deposite>();

    public virtual User IdNavigation { get; set; } = null!;

    public virtual ICollection<Withdrawal> Withdrawals { get; set; } = new List<Withdrawal>();
}
