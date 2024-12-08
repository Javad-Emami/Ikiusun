using Application.Features.ChatModels.GPT_3._5Turbo.Dto;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

namespace Service.Services;

public class OpenAi_ChatGPT3Point5Turbo : IOpenAi_ChatGPT3Point5Turbo
{
    private readonly string _openAIKey;
    public OpenAi_ChatGPT3Point5Turbo(IConfiguration configuration)
    {
        _openAIKey = configuration.GetSection("OpenAI")["Key"];
    }
    public async Task<ChatResponseDto> GetChatCompletion(string text)
    {
        ChatClient client = new(model: "gpt-3.5-turbo", apiKey: _openAIKey);

        var result = await client.CompleteChatAsync(text);
        
        var dto = new ChatResponseDto()
        {
            Content = result.Value.Content[0].Text,
            InputToken = result.Value.Usage.InputTokenCount,
            OutputToken = result.Value.Usage.OutputTokenCount,
        };
        return dto; 
    }

    public async Task<ChatResponseDto> GetChatCompletion(List<ChatMessagesDto> messages)
    {
        ChatClient client = new(model: "gpt-3.5-turbo", apiKey: _openAIKey);

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

        var dto = new ChatResponseDto()
        {
            Content = result.Value.Content[0].Text,
            InputToken = result.Value.Usage.InputTokenCount,
            OutputToken = result.Value.Usage.OutputTokenCount
        };
        return dto;
    }
}
