using Domain.Common;

namespace Domain.Entites;

public class UserRequest : IBaseEntity<Guid>
{
    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }

    public Guid ServiceModelId { get; set; }

    public Guid UserId { get; set; }

    public DateTime RequestTime { get; set; }

    public int InputToken { get; set; }

    public int OutputTokent { get; set; }

    public decimal Cost { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual ServiceModel ServiceModel { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
