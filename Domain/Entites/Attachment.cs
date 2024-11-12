using Domain.Common;

namespace Domain.Entites;

public class Attachment:IBaseEntity<Guid>
{
    public Guid Id { get; set; }

    public Guid MessageId { get; set; }

    public string AttachmentUrl { get; set; } = null!;

    public DateTime CreationDate { get; set; }

    public string FileType { get; set; } = null!;

    public virtual Message Message { get; set; } = null!;
}
