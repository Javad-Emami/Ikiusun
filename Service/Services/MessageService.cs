using Application.Interfaces;
using AutoMapper;
using Domain.Entites;
using Persistance;
using Service.Repository;

namespace Service.Services;

public class MessageService : BaseService<Message, Guid>, IMessageService
{
    public MessageService(AppDbContext db, IMapper mapper) : base(db, mapper)
    {
    }
}
