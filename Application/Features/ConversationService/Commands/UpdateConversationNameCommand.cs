using Application.Features.ConversationService.Dto;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ConversationService.Commands;

public class UpdateConversationNameCommand:IRequest<bool>
{
    public UpdateConversationNameCommand(UpdateConversationNameDto data)
    {
        Data = data;
    }
    public UpdateConversationNameDto Data { get; }
}

public class UpdateConversationNameCommandHandler : IRequestHandler<UpdateConversationNameCommand, bool>
{
    private readonly IConversationService _conversationService;
    public UpdateConversationNameCommandHandler(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }
    public async Task<bool> Handle(UpdateConversationNameCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _conversationService.BaseQuery.Where(a => a.Id == request.Data.Id).FirstOrDefaultAsync();
        if(conversation != null)
            conversation.EditConversationName(request.Data.ConversationName);

        await _conversationService.UpdateAsync(conversation);
        return true;
    }
}
