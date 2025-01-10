using Application.Interfaces;
using AutoMapper;
using Domain.Common;
using Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Persistance;
using Service.Repository;

namespace Service.Services;

public class WalletService : BaseService<WalletTransaction, Guid>, IWalletService
{
    private readonly ISqlDbContext _appDbContext;
    private readonly IUserService _userService;
    public WalletService(SqlDbContext db, IMapper mapper, IUserService userService) : base(db, mapper)
    {
        _appDbContext = db;
        _userService = userService;
    }

    public async Task<bool> HasMinumumBalanceValueForChatModelAsync(string mobile, CancellationToken cancellationToken)
    {
        var user = await _userService.BaseQuery
                    .Include(w => w.WalletTransactions)
                    .Where(u => u.Mobile == mobile)                    
                    .FirstOrDefaultAsync(cancellationToken);
        if (user.WalletTransactions != null)
            if(user.WalletTransactions.OrderByDescending(tt => tt.TransactionTime).Select(b => b.BalanceAmount).FirstOrDefault() <= 5000)
                throw new CustomException(500, "اعتبار شما برای استفاده از این سرویس کافی نمی باشد. لطفا حساب خود را شارژ نمایید.");

        if(user.WalletTransactions.Select(b => b.BalanceAmount).FirstOrDefault() > 500)
            return true;
        return false;
    }

    public async Task<bool> HasMinumumBalanceValueForImageModelAsync(string mobile, CancellationToken cancellationToken)
    {
        var user = await _userService.BaseQuery
                    .Include(w => w.WalletTransactions)
                    .Where(u => u.Mobile == mobile)
                    .FirstOrDefaultAsync(cancellationToken);
        if (user.WalletTransactions != null)
            if (user.WalletTransactions.OrderByDescending(tt => tt.TransactionTime).Select(b => b.BalanceAmount).FirstOrDefault() <= 10000)
                throw new CustomException(500, "اعتبار شما برای استفاده از این سرویس کافی نمی باشد. لطفا حساب خود را شارژ نمایید.");

        if (user.WalletTransactions.Select(b => b.BalanceAmount).FirstOrDefault() > 10000)
            return true;
        return false;
    }
}
