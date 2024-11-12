using Domain.Common;

namespace Domain.Entites;

public class Role: IBaseEntity
{
    public int Id { get; set; }

    public string RoleName { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
