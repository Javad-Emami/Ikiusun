using Microsoft.AspNetCore.Http;

namespace Application.Features.ChatModels.Gpt_4oMini.Dto;

public class Gpt4oMiniRequestDto
{
    public Guid? Id { get; set; }
    public string Text { get; set; }
    public List<IFormFile>? Images { get; set; }
}
