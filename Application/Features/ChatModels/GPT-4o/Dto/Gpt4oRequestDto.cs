using Microsoft.AspNetCore.Http;

namespace Application.Features.ChatModels.GPT_4o.Dto;

public class Gpt4oRequestDto
{
    public Guid? Id { get; set; }
    public string Text { get; set; }
    public List<IFormFile>? Images { get; set; }
}
