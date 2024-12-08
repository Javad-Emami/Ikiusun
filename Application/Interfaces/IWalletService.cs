using Application.Interfaces.Repository;
using Domain.Entites;

namespace Application.Interfaces;

public interface IWalletService: IBaseService<Wallet,Guid>
{
    Task<bool> HasMinumumBalanceValueForChatModel(string mobile,CancellationToken cancellationToken);
    Task<bool> HasMinumumBalanceValueForImageModel(string mobile, CancellationToken cancellationToken);
}
