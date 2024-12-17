using Application.Features.ChatModels.GPT_01Preview.Dto;
using Application.Interfaces.Gpt_01Preview;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

namespace Service.Services.Gpt_01Preview;

public class OpenAi_ChatGPT01Preview : IOpenAi_ChatGPT01Preview
{
    private readonly string _openAIKey;
    public OpenAi_ChatGPT01Preview(IConfiguration configuration)
    {
        _openAIKey = configuration.GetSection("OpenAI")["Key"];
    }
    public async Task<Gpt01PreviewResponseDto> GetChatCompletionAsync(string text)
    {
        ChatClient client = new(model: "o1-preview", apiKey: _openAIKey);

        var result = await client.CompleteChatAsync(text);

        var dto = new Gpt01PreviewResponseDto()
        {
            Content = result.Value.Content[0].Text,
            InputToken = result.Value.Usage.InputTokenCount,
            OutputToken = result.Value.Usage.OutputTokenCount,
        };
        return dto;
    }

    public async Task<Gpt01PreviewResponseDto> GetChatCompletionAsync(List<ChatGpt01PreviewMessagesDto> messages)
    {
        ChatClient client = new(model: "o1-preview", apiKey: _openAIKey);

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

        var dto = new Gpt01PreviewResponseDto()
        {
            Content = result.Value.Content[0].Text,
            InputToken = result.Value.Usage.InputTokenCount,
            OutputToken = result.Value.Usage.OutputTokenCount
        };
        return dto;
    }
}
