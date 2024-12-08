using Domain.Common;
using System.Linq.Expressions;

namespace Application.Interfaces.Repository;

public interface IBaseRepository<TModel, TKey> where TModel : class, IBaseEntity<TKey> where TKey : IEquatable<TKey>

{

    Task<bool> AnyAsync(Expression<Func<TModel?, bool>> expression, CancellationToken cancellationToken = default);

    Task<int> CountAsync(Expression<Func<TModel?, bool>> expression, CancellationToken cancellationToken = default);



    // is exist

    ValueTask<bool> HasAsync(TKey id, CancellationToken cancellationToken = default);

    ValueTask<bool> HasAsync(Expression<Func<TModel?, bool>> filter, CancellationToken cancellationToken = default);



    // get one item

    ValueTask<TModel?> GetAsync(TKey id, CancellationToken cancellationToken = default);



    ValueTask<TProject?> GetAsync<TProject>(TKey id, CancellationToken cancellationToken = default)

        where TProject : class;



    ValueTask<TModel?> GetAsync(Expression<Func<TModel?, bool>> filter, CancellationToken cancellationToken = default);

    ValueTask<TKey?> GetIdAsync(Expression<Func<TModel?, bool>> filter, CancellationToken cancellationToken = default);



    ValueTask<TProject?> GetAsync<TProject>(Expression<Func<TModel?, bool>> filter,

        CancellationToken cancellationToken = default) where TProject : class;



    ValueTask<TProject> GetAsync<TProject>(TKey id, Expression<Func<TModel, TProject>> projection,

        CancellationToken cancellationToken = default)

        where TProject : class;



    ValueTask<TProject?> GetAsync<TProject>(Expression<Func<TModel?, bool>> filter,

        Expression<Func<TModel, TProject>> projection, CancellationToken cancellationToken = default)

        where TProject : class;



    // lists

    ValueTask<IEnumerable<TModel?>> ListAsync(CancellationToken cancellationToken = default);



    ValueTask<IEnumerable<TProject>> ListAsync<TProject>(CancellationToken cancellationToken = default)

        where TProject : class;



    ValueTask<ICollection<TModel?>> ListAsync(Expression<Func<TModel?, bool>> expression,

        CancellationToken cancellationToken = default);



    ValueTask<ICollection<TProject>> ListAsync<TProject>(Expression<Func<TModel?, bool>> expression,

        CancellationToken cancellationToken = default) where TProject : class;



    ValueTask<IEnumerable<TProject>> ListAsync<TProject>(Expression<Func<TModel, TProject>> projection,

        CancellationToken cancellationToken = default)

        where TProject : class;



    ValueTask<IEnumerable<TProject>> ListAsync<TProject>(

        Expression<Func<TModel?, bool>> expression,

        Expression<Func<TModel, TProject>> projection,

        CancellationToken cancellationToken = default

    ) where TProject : class;



    //paged list

    //ValueTask<IPagedList<TModel>> PagedListAsync(Expression<Func<TModel?, bool>> expression, FilterBase filter,

    //    CancellationToken cancellationToken = default);



    //ValueTask<IPagedList<TProject>> PagedListAsync<TProject>(Expression<Func<TModel?, bool>> expression,

    //    FilterBase filter, CancellationToken cancellationToken = default)

    //    where TProject : class;



    //ValueTask<IPagedList<TModel>> PagedListAsync(FilterBase filter,

    //    CancellationToken cancellationToken = default);



    //ValueTask<IPagedList<TProject>> PagedListAsync<TProject>(FilterBase filter,

    //    CancellationToken cancellationToken = default) where TProject : class;



    //ValueTask<IPagedList<TProject>> PagedListAsync<TProject>(

    //    Expression<Func<TModel?, bool>> expression,

    //    Expression<Func<TModel, TProject>> projection, FilterBase filter,

    //    CancellationToken cancellationToken = default

    //) where TProject : class;



    //ValueTask<IPagedList<TProject>> PagedListAsync<TProject>(

    //    Expression<Func<TModel, TProject>> projection, FilterBase filter,

    //    CancellationToken cancellationToken = default

    //) where TProject : class;





    Task<TModel> AddAsync(TModel? model, CancellationToken cancellationToken = default);

    Task AddAsync(IEnumerable<TModel?> model, CancellationToken cancellationToken = default);



    Task UpdateAsync(TModel? model, CancellationToken cancellationToken = default);

    Task UpdateRangeAsync(IEnumerable<TModel?> items, CancellationToken cancellationToken = default);



    Task RemoveAsync(TKey id, CancellationToken cancellationToken = default);

    Task RemoveAsync(TModel? model, CancellationToken cancellationToken = default);

    Task RemoveRangeAsync(IEnumerable<TModel?> models, CancellationToken cancellationToken = default);

}

public interface IBaseRepository<TModel> : IBaseRepository<TModel, int> where TModel: class, IBaseEntity
{
}
