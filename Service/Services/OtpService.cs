using Application.Interfaces;
using AutoMapper;
using Domain.Common;
using Microsoft.Extensions.Configuration;
using OtpNet;
using System.Text;

namespace Service.Services;

public class OtpService: IOtpService
{
    private readonly IUserService _userService;
    private readonly string _apiKey;
    private readonly string senderNumber;
    private readonly Random _random = new Random();
    private readonly IMapper _mapper;
    public OtpService(IMapper mapper, IUserService userService, IConfiguration configuration) 
    {
        _userService = userService;
        _apiKey = configuration.GetSection("Otp")["ApiKey"];
        senderNumber = configuration.GetSection("Otp")["SendNumber"];
        _mapper = mapper;
    }

    public async Task<bool> SendSms(string mobileNumber,CancellationToken cancellationToken)
    {
        try
        {
            byte[] secretKey = Encoding.UTF32.GetBytes(mobileNumber);
            var totp = new Totp(secretKey, mode: OtpHashMode.Sha512, step: 120, totpSize: 6);            
            var totpCode = totp.ComputeTotp();
            return true;
        }
        catch (Exception ex)
        {
            throw new CustomException(500,ex.Message.ToString());
        }
        
    }
}
