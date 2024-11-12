using Domain.Common;

namespace Domain.Entites;

public class Otp : IBaseEntity<Guid>
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public int OtpCode { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime ExpireTime { get; set; }

    public bool IsUsed { get; set; }

    public virtual User User { get; set; } = null!;
}
