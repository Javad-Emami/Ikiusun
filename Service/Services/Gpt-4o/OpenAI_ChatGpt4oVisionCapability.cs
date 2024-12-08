using Application.Features.ChatModels.GPT_4o.Dto;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

namespace Service.Services.Gpt_4o;

public class OpenAI_ChatGpt4oVisionCapability : IOpenAI_ChatGPT4oVisionCapability
{
    private readonly string _openAIKey;
    public OpenAI_ChatGpt4oVisionCapability(IConfiguration configuration)
    {
        _openAIKey = configuration.GetSection("OpenAI")["Key"];
    }
    public async Task<Gpt4oResponseDto> GetChatCompletion(string text)
    {
        ChatClient client = new(model: "gpt-4o", apiKey: _openAIKey);

        var result = await client.CompleteChatAsync(text);

        var dto = new Gpt4oResponseDto()
        {
            Content = result.Value.Content[0].Text,
            InputToken = result.Value.Usage.InputTokenCount,
            OutputToken = result.Value.Usage.OutputTokenCount,
        };
        return dto;
    }

    public async Task<Gpt4oResponseDto> GetChatCompletion(List<ChatGpt4oMessagesDto> messages)
    {
        ChatClient client = new(model: "gpt-4o", apiKey: _openAIKey);

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

        var dto = new Gpt4oResponseDto()
        {
            Content = result.Value.Content[0].Text,
            InputToken = result.Value.Usage.InputTokenCount,
            OutputToken = result.Value.Usage.OutputTokenCount
        };
        return dto;
    }

    public async Task<Gpt4oResponseDto> GetChatCompletionWithVision(List<IFormFile> images, string prompt)
    {
        throw new NotImplementedException();
    }
}
