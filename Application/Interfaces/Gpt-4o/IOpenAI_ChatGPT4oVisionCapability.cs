using Application.Features.ChatModels.GPT_3._5Turbo.Dto;
using Application.Features.ChatModels.GPT_4o.Dto;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

public interface IOpenAI_ChatGPT4oVisionCapability
{
    Task<Gpt4oResponseDto> GetChatCompletion(string text);

    Task<Gpt4oResponseDto> GetChatCompletion(List<ChatGpt4oMessagesDto> messages);

    Task<Gpt4oResponseDto> GetChatCompletionWithVision(List<IFormFile> images, string prompt);
}
