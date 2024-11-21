using Application.Interfaces.Repository;
using Domain.Entites;

namespace Application.Interfaces;

public interface IConversationService: IBaseService<Conversation,Guid>
{
}
