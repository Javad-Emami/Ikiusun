using AutoMapper;

namespace Application.Features.Account.Dto;

public class UserDto
{
    public Guid Id { get; set; }
    public string Mobile { get; set; }
}

public class UserDtoMapper:Profile
{
    public UserDtoMapper()
    {
        CreateMap<Domain.Entites.User,UserDto>().ReverseMap();
    }
}
