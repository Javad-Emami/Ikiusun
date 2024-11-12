using Application.Interfaces;
using MediatR;

namespace Application.Features.ChatModels.GPT_3._5Turbo.Query;

public class GPTThreePointFiveTurboQuery: IRequest<string>
{
    public GPTThreePointFiveTurboQuery(string text)
    {
        Text = text;
    }
    public string Text { get; }
}

public class GPTThreePointFiveTurboQueryHandler : IRequestHandler<GPTThreePointFiveTurboQuery, string>
{
    private readonly IOpenAi_ChatGPT _openAi_ChatGpt;
    public GPTThreePointFiveTurboQueryHandler(IOpenAi_ChatGPT openAi_ChatGpt)
    {
        _openAi_ChatGpt = openAi_ChatGpt;
    }
    public async Task<string> Handle(GPTThreePointFiveTurboQuery request, CancellationToken cancellationToken)
    {
        return await _openAi_ChatGpt.GetChatCompletion(request.Text);
    }
}
