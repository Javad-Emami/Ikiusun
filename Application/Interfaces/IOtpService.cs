using Application.Features.Otp.Dto;
using Application.Interfaces.Repository;
using Domain.Entites;

namespace Application.Interfaces;

public interface IOtpService :IBaseService<Otp, Guid>
{
    Task<string> GenerateOtp(CancellationToken cancellationToken);
    Task<OtpDto> SendSms(string mobileNumber, CancellationToken cancellationToken);
}
