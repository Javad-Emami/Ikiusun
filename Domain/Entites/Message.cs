using Domain.Common;

namespace Domain.Entites;

public class Message : IBaseEntity<Guid>
{
    public Message(Guid conversationId, string content, int senderType)
    {
        ConversationId = conversationId;
        Content = content;
        SenderType = senderType;
    }
    public Guid Id { get; set; }

    public Guid ConversationId { get; private set; }

    public int SenderType { get; private set; }

    public string Content { get; private set; }

    public DateTime CreationDate { get; private set; } = DateTime.Now;

    public int SequenceNumber { get; set; }

    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual SenderType SenderTypeNavigation { get; set; } = null!;

}
