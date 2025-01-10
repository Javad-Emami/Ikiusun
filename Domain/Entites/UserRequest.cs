using Domain.Common;

namespace Domain.Entites;

public class UserRequest : IBaseEntity<Guid>
{
    public UserRequest(Guid userId, Guid conversationId,int serviceModelId,int? inputToken, int? outputTokent)
    {
        UserId = userId;
        ConversationId = conversationId;
        ServiceModelId = serviceModelId;
        InputToken = inputToken;
        OutputTokent = outputTokent;
    }

    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }

    public int ServiceModelId { get; set; }

    public Guid UserId { get; set; }

    public DateTime RequestTime { get; set; } = DateTime.Now;

    public int? InputToken { get; set; }

    public int? OutputTokent { get; set; }

    public decimal Cost { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual ServiceModel ServiceModel { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
