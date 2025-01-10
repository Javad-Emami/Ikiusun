using Domain.Common;

namespace Domain.Entites;

public class User : IBaseEntity<Guid>
{
    public User(string mobile)
    {
        Mobile = mobile;
    }
    public Guid Id { get; set; }

    public string Mobile { get; set; } = null!;

    public DateTime CreationDate { get; set; } = DateTime.Now;

    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    public virtual ICollection<UserRequest> UserRequests { get; set; } = new List<UserRequest>();

    public virtual ICollection<WalletTransaction> WalletTransactions { get; set; } = new List<WalletTransaction>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
