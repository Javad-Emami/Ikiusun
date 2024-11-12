using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using OpenAI_API;
using OpenAI_API.Models;

namespace Service.Services;

public class OpenAi_ChatGpt : IOpenAi_ChatGPT
{
    private readonly string _openAIKey;
    public OpenAi_ChatGpt(IConfiguration configuration)
    {
        _openAIKey = configuration.GetSection("OpenAI")["Key"];
    }
    public async Task<string> GetChatCompletion(string text)
    {
        ChatClient client = new(model: "gpt-3.5-turbo", apiKey: _openAIKey);

        var result = await client.CompleteChatAsync(text);
        var tokens = result.Value.Usage.TotalTokenCount;
        return result.Value.Content[0].Text;

        //var api = new OpenAI(_openAIKey);
        //var model = Model.ChatGPTTurbo_1106;
        //var result = await api.Completions.CreateCompletionAsync(new OpenAI_API.Completions.CompletionRequest(text,model: model,temperature:0.1));
        //var tokens = result.Usage.TotalTokens;
        //return result.Completions[0].Text;
    }
}
