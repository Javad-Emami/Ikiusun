namespace Application.Features.ChatModels.GPT_01Preview.Dto;

public class Gpt01PreviewResponseDto
{
    public Guid ConversationId { get; set; }
    public string Content { get; set; }
    public int InputToken { get; set; }
    public int OutputToken { get; set; }
}
