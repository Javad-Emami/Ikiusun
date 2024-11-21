using Application.Interfaces;
using AutoMapper;
using Domain.Entites;
using Persistance;
using Service.Repository;

namespace Service.Services;

public class ConversationService : BaseService<Conversation, Guid>, IConversationService
{
    public ConversationService(AppDbContext db, IMapper mapper) : base(db, mapper)
    {
    }
}
