using Application.Interfaces;
using AutoMapper;
using Domain.Entites;
using Persistance;
using Service.Repository;

namespace Service.Services;

public class RoleService : BaseService<Role>, IRoleService
{
    public RoleService(SqlDbContext context, IMapper mapper) : base(context, mapper)
    {
    }
}
