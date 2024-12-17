using Application.Features.ChatModels.GPT_4o.Dto;
using Application.Features.ChatModels.Gpt_4oMini.Dto;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Gpt_4oMini;

public interface IOpenAI_ChatGPT4oMiniVisionCapability
{
    Task<Gpt4oMiniResponseDto> GetChatCompletionAsync(string text);

    Task<Gpt4oMiniResponseDto> GetChatCompletionAsync(List<ChatGpt4oMiniMessagesDto> messages);

    Task<Gpt4oMiniResponseDto> GetChatCompletionWithVisionAsync(List<IFormFile> images, string prompt);
}
