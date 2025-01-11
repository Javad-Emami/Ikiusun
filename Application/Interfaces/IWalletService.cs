using Application.Interfaces.Repository;
using Domain.Entites;

namespace Application.Interfaces;

public interface IWalletService: IBaseService<WalletTransaction,Guid>
{
    Task<bool> HasMinumumBalanceValueForChatModelAsync(string mobile,CancellationToken cancellationToken);
    Task<bool> HasMinumumBalanceValueForImageModelAsync(string mobile, CancellationToken cancellationToken);
    Task<bool> UpdateUserWalletBalance(Guid userId, decimal costUsage,CancellationToken cancellationToken);
}
