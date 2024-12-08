namespace Application.Features.ChatModels.GPT_4o.Dto;

public class Gpt4oResponseDto
{
    public Guid ConversationId { get; set; }
    public string Content { get; set; }
    public int InputToken { get; set; }
    public int OutputToken { get; set; }
}
