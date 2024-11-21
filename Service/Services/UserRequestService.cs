using Application.Interfaces;
using AutoMapper;
using Domain.Entites;
using Persistance;
using Service.Repository;

namespace Service.Services;

public class UserRequestService : BaseService<UserRequest, Guid>, IUserRequestService
{
    public UserRequestService(AppDbContext db, IMapper mapper) : base(db, mapper)
    {
    }
}
