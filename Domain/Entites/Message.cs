using Domain.Common;

namespace Domain.Entites;

public class Message : IBaseEntity<Guid>
{
    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }

    public int SenderType { get; set; }

    public string Content { get; set; } = null!;

    public DateTime CreationDate { get; set; }

    public int SequenceNumber { get; set; }

    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual SenderType SenderTypeNavigation { get; set; } = null!;
}
