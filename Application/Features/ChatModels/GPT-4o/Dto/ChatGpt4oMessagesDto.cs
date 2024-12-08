using AutoMapper;
using Domain.Entites;

namespace Application.Features.ChatModels.GPT_4o.Dto;

public class ChatGpt4oMessagesDto
{
    public int? SenderTypeId { get; set; }
    public string Content { get; set; }
}

public class ChatGpt4oMessagesDtoMapper :Profile
{
    public ChatGpt4oMessagesDtoMapper()
    {
        CreateMap<Message, ChatGpt4oMessagesDto>().ReverseMap();
    }
}
