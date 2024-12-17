using AutoMapper;
using Domain.Entites;

namespace Application.Features.ChatModels.GPT_01Preview.Dto;

public class ChatGpt01PreviewMessagesDto
{
    public int? SenderTypeId { get; set; }
    public string Content { get; set; }
}

public class ChatGpt01PreviewMessagesDtoMapper: Profile
{
    public ChatGpt01PreviewMessagesDtoMapper()
    {
        CreateMap<Message,ChatGpt01PreviewMessagesDto>().ReverseMap();
    }
}