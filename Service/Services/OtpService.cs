using Application.Features.Otp.Dto;
using Application.Interfaces;
using AutoMapper;
using Domain.Common;
using Domain.Entites;
using Microsoft.Extensions.Configuration;
using Persistance;
using Service.Repository;

namespace Service.Services;

public class OtpService : BaseService<Otp, Guid>, IOtpService
{
    private readonly IUserService _userService;
    private readonly string _apiKey;
    private readonly string senderNumber;
    private readonly Random _random = new Random();
    private readonly IMapper _mapper;
    public OtpService(AppDbContext db, IMapper mapper, IUserService userService, IConfiguration configuration) : base(db, mapper)
    {
        _userService = userService;
        _apiKey = configuration.GetSection("Otp")["ApiKey"];
        senderNumber = configuration.GetSection("Otp")["SendNumber"];
        _mapper = mapper;
    }

    public async Task<string> GenerateOtp(CancellationToken cancellationToken)
    {
        int length = 6;
        return _random.Next((int)Math.Pow(10, length - 1), (int)Math.Pow(10, length)).ToString("D" + length);
    }

    public async Task<OtpDto> SendSms(string mobileNumber,CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.GetAsync(m => m.Mobile == mobileNumber);

            if (user == null)
                throw new CustomException(500, "کاربر یافت نشد");

            var otpCode = Int32.Parse(await GenerateOtp(cancellationToken));
            var newOtp = new Otp()
            {
                OtpCode = otpCode,
                CreationTime = DateTime.Now,
                ExpireTime = DateTime.Now.AddMinutes(2),
                UserId = user.Id,
                IsUsed = false,
            };

           // var api = new Kavenegar.KavenegarApi(_apiKey);

           // await api.Send(senderNumber, mobileNumber, newOtp.OtpCode.ToString());

            return _mapper.Map<OtpDto>(newOtp);
        }
        catch (Exception ex)
        {

            throw new CustomException(500,ex.Message.ToString());
        }
        
    }
}
