using Application.Features.ChatModels.GPT_3._5Turbo.Dto;

namespace Application.Interfaces;

public interface IOpenAi_ChatModel
{
    Task<ChatResponseDto> GetChatCompletion(string text);

    Task<ChatResponseDto> GetChatCompletion(List<ChatMessagesDto> messages);
}
