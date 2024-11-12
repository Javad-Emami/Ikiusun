using Domain.Common;

namespace Domain.Entites;

public class User : IBaseEntity<Guid>
{
    public Guid Id { get; set; }

    public string Mobile { get; set; } = null!;

    public DateTime CreationDate { get; set; }

    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    public virtual ICollection<Otp> Otps { get; set; } = new List<Otp>();

    public virtual ICollection<UserRequest> UserRequests { get; set; } = new List<UserRequest>();

    public virtual Wallet? Wallet { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
