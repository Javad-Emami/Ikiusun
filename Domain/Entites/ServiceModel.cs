using Domain.Common;

namespace Domain.Entites;

public class ServiceModel : IBaseEntity<Guid>
{
    public Guid Id { get; set; }

    public string SeviceName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreationDate { get; set; }

    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    public virtual ICollection<Pricing> Pricings { get; set; } = new List<Pricing>();

    public virtual ICollection<UserRequest> UserRequests { get; set; } = new List<UserRequest>();
}
