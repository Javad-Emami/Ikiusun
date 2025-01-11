using Domain.Common;
using Domain.Enums;

namespace Domain.Entites;

public class WalletTransaction : IBaseEntity<Guid>
{
    public WalletTransaction()
    {
        
    }
    public WalletTransaction(Guid userId, int transactionTypeId, decimal transactionAmount,decimal lastBalanceAmount)
    {
        UserId = userId;
        TransactionTypeId = (int)transactionTypeId;
        TransactionAmount = transactionAmount;
        BalanceAmount = lastBalanceAmount - transactionAmount > 0 ? lastBalanceAmount - transactionAmount : 0;
    }
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public int TransactionTypeId { get; set; } = 2;

    public DateTime TransactionTime { get; private set; } = DateTime.Now;

    public decimal TransactionAmount { get; set; }

    public decimal BalanceAmount { get; set; }

    //public virtual TransactionType TransactionType { get; set; } 

    public virtual User User { get; set; } = null!;
}
