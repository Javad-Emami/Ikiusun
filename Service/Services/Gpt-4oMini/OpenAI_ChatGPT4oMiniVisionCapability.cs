using Application.Features.ChatModels.GPT_4o.Dto;
using Application.Features.ChatModels.Gpt_4oMini.Dto;
using Application.Interfaces.Gpt_4oMini;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

namespace Service.Services.Gpt_4oMini;

public class OpenAI_ChatGPT4oMiniVisionCapability : IOpenAI_ChatGPT4oMiniVisionCapability
{
    private readonly string _openAIKey;
    public OpenAI_ChatGPT4oMiniVisionCapability(IConfiguration configuration)
    {
        _openAIKey = configuration.GetSection("OpenAI")["Key"];
    }
    public async Task<Gpt4oMiniResponseDto> GetChatCompletion(string text)
    {
        ChatClient client = new(model: "gpt-4o-mini-2024-07-18", apiKey: _openAIKey);

        var result = await client.CompleteChatAsync(text);

        var dto = new Gpt4oMiniResponseDto()
        {
            Content = result.Value.Content[0].Text,
            InputToken = result.Value.Usage.InputTokenCount,
            OutputToken = result.Value.Usage.OutputTokenCount,
        };
        return dto;
    }

    public async Task<Gpt4oMiniResponseDto> GetChatCompletion(List<ChatGpt4oMiniMessagesDto> messages)
    {
        ChatClient client = new(model: "gpt-4o-mini-2024-07-18", apiKey: _openAIKey);

        var messagesHistory = new List<ChatMessage>();
        foreach (var message in messages)
        {
            if (message.SenderTypeId == (int)SenderTypeEnum.user)
                messagesHistory.Add(new UserChatMessage(message.Content));
            if (message.SenderTypeId == (int)SenderTypeEnum.assistant)
                messagesHistory.Add(new AssistantChatMessage(message.Content));
            if (message.SenderTypeId == null)
                messagesHistory.Add(new UserChatMessage(message.Content));
        }

        var result = await client.CompleteChatAsync(messagesHistory);

        var dto = new Gpt4oMiniResponseDto()
        {
            Content = result.Value.Content[0].Text,
            InputToken = result.Value.Usage.InputTokenCount,
            OutputToken = result.Value.Usage.OutputTokenCount
        };
        return dto;
    }

    public async Task<Gpt4oMiniResponseDto> GetChatCompletionWithVision(List<IFormFile> images, string prompt)
    {
        throw new NotImplementedException();
    }
}
