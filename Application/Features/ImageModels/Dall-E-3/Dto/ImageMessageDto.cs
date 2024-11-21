using AutoMapper;
using Domain.Entites;

namespace Application.Features.ImageModels.Dall_E_3.Dto;

public class MessagesDto
{
    public int? SenderTypeId { get; set; }
    public string Content { get; set; }
}

public class MessagesDtoMapper : Profile
{
    public MessagesDtoMapper()
    {
        CreateMap<Message, MessagesDto>().ReverseMap();
    }
}
