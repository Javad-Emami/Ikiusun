using Application.Interfaces;
using AutoMapper;
using Domain.Entites;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.Queries;

public class GetUserQuery: IRequest<bool>
{
    public GetUserQuery(string mobileNumber)
    {
        MobileNumber = mobileNumber;    
    }
    public string MobileNumber { get; }
}

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, bool>
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly IOtpService _otpService;
    private readonly IRoleService _roleService;
    private readonly IWalletService _walletService;
    public GetUserQueryHandler(IUserService userService, IMapper mapper, IOtpService otpService, IRoleService roleService, IWalletService walletService)
    {
        _userService = userService;
        _mapper = mapper;
        _otpService = otpService;
        _roleService = roleService;
        _walletService = walletService;
    }
    public async Task<bool> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userService.BaseQuery.Where(m => m.Mobile == request.MobileNumber).FirstOrDefaultAsync(cancellationToken);
        if (user != null)
        {
            var otp = await _otpService.SendSms(request.MobileNumber,cancellationToken);
            return true;
        }
        else
        {
            var newUser = new Domain.Entites.User(request.MobileNumber);

            var role = await _roleService.BaseQuery.Where(r => r.RoleName == RoleEnum.User.ToString()).FirstOrDefaultAsync();
            newUser.Roles.Add(role);
            await _userService.AddAsync(newUser);
            
            var newUserGift = new WalletTransaction(newUser.Id, 1, 0, 10000);
            await _walletService.AddAsync(newUserGift);
            
            var otp = await _otpService.SendSms(request.MobileNumber,cancellationToken);
            return true;
        }        
    }
}