using Domain.Common;

namespace Application.Interfaces.Repository;

public interface IBaseService<T, TKey>: IEFBaseRepository<T, TKey>
    where T : class,IBaseEntity<TKey> where TKey : IEquatable<TKey>
{
}

public interface IBaseService<T>: IBaseService<T,int> where T : class,IBaseEntity
{

}
