using AutoMapper;
using Domain.Entites;

namespace Application.Features.ChatModels.Gpt_4oMini.Dto;

public class ChatGpt4oMiniMessagesDto
{
    public int? SenderTypeId { get; set; }
    public string Content { get; set; }
}

public class ChatGpt4oMiniMessagesDtoMapper:Profile
{
    public ChatGpt4oMiniMessagesDtoMapper()
    {
        CreateMap<Message, ChatGpt4oMiniMessagesDto>().ReverseMap();
    }
}
