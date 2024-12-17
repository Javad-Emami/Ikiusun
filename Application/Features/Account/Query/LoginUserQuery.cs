using Application.Features.Account.Dto;
using Application.Interfaces;
using AutoMapper;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using System.Text;

namespace Application.Features.Account.Query;

public class LoginUserQuery:IRequest<UserDto>
{
    public LoginUserQuery(LoginDto data)
    {
        Data = data;
    }
    public LoginDto Data { get;}
}

public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, UserDto>
{
    private readonly IUserService _userService;
    private readonly IOtpService _otpService;
    private readonly IMapper _mapper;
    public LoginUserQueryHandler(IUserService userService, IOtpService otpService, IMapper mapper)
    {
        _userService = userService;
        _otpService = otpService;
        _mapper = mapper;
    }
    public async Task<UserDto> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetAsync(u => u.Mobile == request.Data.Mobile);
        byte[] secretKey = Encoding.UTF32.GetBytes(user.Mobile);
        var totp = new Totp(secretKey, mode: OtpHashMode.Sha512, step: 120, totpSize: 6);

        if (totp.VerifyTotp(request.Data.OtpCode, out long timeWindowUsed))
        {
            return _mapper.Map<UserDto>(user);
        }

        throw new CustomException(500,"احراز هویت انجام نشد");
    }
}