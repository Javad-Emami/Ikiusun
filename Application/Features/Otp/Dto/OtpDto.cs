using AutoMapper;
using Domain.Entites;

namespace Application.Features.Otp.Dto;

public class OtpDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public int OtpCode { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime ExpireTime { get; set; }

    public bool IsUsed { get; set; }
}

public class OtpDtoMapper:Profile
{
    public OtpDtoMapper()
    {
        CreateMap<Domain.Entites.Otp,OtpDto>().ReverseMap();
    }
}