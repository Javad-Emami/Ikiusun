using Application.Interfaces;
using AutoMapper;
using Domain.Common;
using Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Persistance;
using Service.Repository;

namespace Service.Services;

public class WalletService : BaseService<Wallet, Guid>, IWalletService
{
    private readonly IAppDbContext _appDbContext;
    private readonly IUserService _userService;
    public WalletService(AppDbContext db, IMapper mapper, IUserService userService) : base(db, mapper)
    {
        _appDbContext = db;
        _userService = userService;
    }

    public async Task<bool> HasMinumumBalanceValueForChatModelAsync(string mobile, CancellationToken cancellationToken)
    {
        var user = await _userService.BaseQuery
                    .Include(w => w.Wallet)
                    .Where(u => u.Mobile == mobile)
                    .FirstOrDefaultAsync(cancellationToken);
        if (user.Wallet == null)
            throw new CustomException(500, "اعتبار شما برای استفاده از این سرویس کافی نمی باشد. لطفا حساب خود را شارژ نمایید.");

        if(user!.Wallet!.BalanceAmount > (decimal)500)
            return true;
        return false;
    }

    public async Task<bool> HasMinumumBalanceValueForImageModelAsync(string mobile, CancellationToken cancellationToken)
    {
        var user = await _userService.BaseQuery
                    .Include(w => w.Wallet)
                    .Where(u => u.Mobile == mobile)
                    .FirstOrDefaultAsync(cancellationToken);
        if (user.Wallet == null)
            throw new CustomException(500, "اعتبار شما برای استفاده از این سرویس کافی نمی باشد. لطفا حساب خود را شارژ نمایید.");

        if (user!.Wallet!.BalanceAmount > (decimal)10000)
            return true;
        return false;
    }
}
