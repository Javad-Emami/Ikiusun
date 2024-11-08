using Domain.Common;

namespace Domain.Entites;

public partial class User : IBaseEntity<Guid>
{
    public Guid Id { get; set; }

    public byte Mobile { get; set; }

    public DateTime CreationDate { get; set; }

    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    public virtual ICollection<Otp> Otps { get; set; } = new List<Otp>();

    public virtual ICollection<UserRequest> UserRequests { get; set; } = new List<UserRequest>();

    public virtual Wallet? Wallet { get; set; }
}
