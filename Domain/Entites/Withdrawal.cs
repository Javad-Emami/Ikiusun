namespace Domain.Entites;

public class Withdrawal
{
    public Guid Id { get; set; }

    public Guid WalletId { get; set; }

    public decimal WithdrawalAmount { get; set; }

    public DateTime WithdrawalDate { get; set; }

    public virtual Wallet Wallet { get; set; } = null!;
}
