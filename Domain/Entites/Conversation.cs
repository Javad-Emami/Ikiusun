using Domain.Common;

namespace Domain.Entites;

public class Conversation : IBaseEntity<Guid>
{
    public Conversation()
    {

    }

    public Conversation(Guid userId, int serviceModelId)
    {
        UserId = userId;
        ServiceModelId = serviceModelId;
    }

    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int ServiceModelId { get; set; }

    public string? ConversationName { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ServiceModel ServiceModel { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual ICollection<UserRequest> UserRequests { get; set; } = new List<UserRequest>();

    public void EditConversationName(string conversationName)
    {
        ConversationName = conversationName;
    }

    public void AddNewMessage(List<Message> messages)
    {
        foreach (var message in messages) 
        {
            Messages.Add(message);            
        }
    }

    public void AddUserRequest(UserRequest userRequest)
    {
        UserRequests.Add(userRequest);
    }
}
