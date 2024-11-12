using AutoMapper;
using Domain.Common;
using Persistance;

namespace Service.Repository;

public class BaseService<T, TKey> : EFBaseRepository<AppDbContext, T, TKey>
    where TKey : IEquatable<TKey> where T : class, IBaseEntity<TKey>
{
    public BaseService(AppDbContext db, IMapper mapper) : base(db, mapper)
    {
    }
}

public abstract class BaseService<TEntity>: BaseService<TEntity,int> where TEntity : class, IBaseEntity<int>
{
    protected BaseService(AppDbContext context, IMapper mapper): base(context, mapper) 
    {
    }
}
