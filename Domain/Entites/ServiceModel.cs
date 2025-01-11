using Domain.Common;

namespace Domain.Entites;

public class ServiceModel
{
    public int Id { get; set; }

    public string SeviceName { get; set; } = null!;

    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    public virtual ICollection<Pricing> Pricings { get; set; } = new List<Pricing>();

    public virtual ICollection<UserRequest> UserRequests { get; set; } = new List<UserRequest>();
}
