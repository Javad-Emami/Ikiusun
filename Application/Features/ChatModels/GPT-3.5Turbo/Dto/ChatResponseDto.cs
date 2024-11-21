namespace Application.Features.ChatModels.GPT_3._5Turbo.Dto;

public class ChatResponseDto
{
    public string Content { get; set; }
    public int InputToken { get; set; }
    public int OutputToken { get; set; }
}
