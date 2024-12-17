using Application.Features.ChatModels.GPT_01Preview.Dto;

namespace Application.Interfaces.Gpt_01Preview;

public interface IOpenAi_ChatGPT01Preview
{
    Task<Gpt01PreviewResponseDto> GetChatCompletionAsync(string text);

    Task<Gpt01PreviewResponseDto> GetChatCompletionAsync(List<ChatGpt01PreviewMessagesDto> messages);
}
