using AutoMapper;
using Domain.Entites;

namespace Application.Features.ChatModels.GPT_3._5Turbo.Dto;

public class ChatMessagesDto
{
    public int? SenderTypeId { get; set; }
    public string Content { get; set; }
}

public class MessagesDtoMapper: Profile
{
    public MessagesDtoMapper()
    {
        CreateMap<Message,ChatMessagesDto>().ReverseMap();
    }
}