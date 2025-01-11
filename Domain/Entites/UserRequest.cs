using Domain.Common;

namespace Domain.Entites;

public class UserRequest : IBaseEntity<Guid>
{
    public UserRequest()
    {
            
    }
    public UserRequest(Guid userId, Guid conversationId,int? inputToken, int? outputTokent, decimal cost, Guid pricingId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        ConversationId = conversationId;
        InputToken = inputToken;
        OutputTokent = outputTokent;
        Cost = cost;
        PricingId = pricingId;
    }

    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }

    public Guid UserId { get; set; }

    public DateTime RequestTime { get; set; } = DateTime.Now;

    public int? InputToken { get; set; }

    public int? OutputTokent { get; set; }

    public decimal Cost { get; set; }

    public Guid PricingId { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual Pricing Pricing { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
