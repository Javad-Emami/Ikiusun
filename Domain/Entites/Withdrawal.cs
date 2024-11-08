using System;
using System.Collections.Generic;

namespace Domain.Entites;

public partial class Withdrawal
{
    public Guid Id { get; set; }

    public Guid WalletId { get; set; }

    public decimal WithdrawalAmount { get; set; }

    public DateTime WithdrawalDate { get; set; }

    public virtual Wallet Wallet { get; set; } = null!;
}
