namespace Application.Interfaces;

public interface IOpenAi_ChatGPT
{
    Task<string> GetChatCompletion(string text);
}
