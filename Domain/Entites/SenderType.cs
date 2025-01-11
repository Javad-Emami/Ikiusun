using Domain.Common;

namespace Domain.Entites;

public class SenderType 
{
    public int Id { get; set; }

    public string SenderType1 { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
