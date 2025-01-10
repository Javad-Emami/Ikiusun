using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Conversation.Commads;

public class DeleteConversationCommand:IRequest<bool>
{
    public DeleteConversationCommand(Guid conversationId)
    {
        ConversationId = conversationId;
    }
    public Guid ConversationId { get; }
}

public class DeleteConversationCommandHandler : IRequestHandler<DeleteConversationCommand, bool>
{
    private readonly IConversationService _conversationService;
    private readonly IMessageService _messageService;
    private readonly ISqlDbContext _appDbContext;
    public DeleteConversationCommandHandler(IConversationService conversationService, IMessageService messageService, ISqlDbContext appDbContext)
    {
        _conversationService = conversationService;
        _messageService = messageService;
        _appDbContext = appDbContext;
    }
    public async Task<bool> Handle(DeleteConversationCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = await _appDbContext.datbase.BeginTransactionAsync(cancellationToken);
        try
        {
            var conversation = await _conversationService.GetAsync(request.ConversationId);
            var relatedMessages = await _messageService.BaseQuery.Where(m => m.ConversationId == request.ConversationId).ToListAsync(cancellationToken);
            await _messageService.RemoveRangeAsync(relatedMessages);
            await _conversationService.RemoveAsync(conversation);
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new CustomException(500, "هنگام حذف گفتگو عملیات با خطا مواجه شد");
        }
        
    }
}
