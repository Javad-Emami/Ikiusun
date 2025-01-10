using Domain.Common;
using Domain.Enums;

namespace Domain.Entites;

public class WalletTransaction : IBaseEntity<Guid>
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public int TransactionTypeId { get; set; }

    public DateTime TransactionTime { get; set; }

    public decimal TransactionAmount { get; set; }

    public decimal BalanceAmount { get; set; }

    //public virtual TransactionType TransactionType { get; set; } 

    public virtual User User { get; set; } = null!;
}
