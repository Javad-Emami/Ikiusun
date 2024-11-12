using Domain.Common;

namespace Domain.Entites;

public class Conversation : IBaseEntity<Guid>
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid ServiceModelId { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime EndedAt { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ServiceModel ServiceModel { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual ICollection<UserRequest> UserRequests { get; set; } = new List<UserRequest>();
}
