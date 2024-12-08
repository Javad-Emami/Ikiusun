namespace Application.Features.ChatModels.Gpt_4oMini.Dto;

public class Gpt4oMiniResponseDto
{
    public Guid ConversationId { get; set; }
    public string Content { get; set; }
    public int InputToken { get; set; }
    public int OutputToken { get; set; }
}
