using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces.Repository;

public interface IEFBaseRepository<TModel, TKey>: IBaseRepository<TModel, TKey>
    where TModel : class, IBaseEntity<TKey> where TKey : IEquatable<TKey>
{
    DbSet<TModel> Set {  get; }
    IQueryable<TModel> BaseQuery { get; }
}

public interface IEFBaseRepository<TModel>: IEFBaseRepository<TModel, int> where TModel : class, IBaseEntity
{

}