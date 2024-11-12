using Application.Features.User.Dto;
using Application.Interfaces;
using AutoMapper;
using Azure.Core;
using Domain.Entites;
using Persistance;
using Service.Repository;

namespace Service.Services;

public class UserService : BaseService<User, Guid>, IUserService
{
    public UserService(AppDbContext db, IMapper mapper) : base(db, mapper)
    {
    }
}
