using Application.Features.ChatModels.GPT_3._5Turbo.Dto;

namespace Application.Interfaces;

public interface IOpenAi_ChatGPT3Point5Turbo
{
    Task<ChatResponseDto> GetChatCompletionAsync(string text);

    Task<ChatResponseDto> GetChatCompletionAsync(List<ChatMessagesDto> messages);
}
