using Application.Interfaces.Repository;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Service.Repository;

public abstract class EFBaseRepository<TDbContext, TModel, TKey> : IEFBaseRepository<TModel, TKey>
    where TDbContext : DbContext
    where TModel : class, IBaseEntity<TKey>
    where TKey : IEquatable<TKey>
{
    private readonly IMapper _mapper;
    protected EFBaseRepository(TDbContext db, IMapper mapper)
    {
        Db = db;
        _mapper= mapper;
        Set = db.Set<TModel>();
    }

    public DbSet<TModel> Set { get; }
    public TDbContext Db { get; }
    public virtual IQueryable<TModel> BaseQuery => Set.AsQueryable();

    public virtual async Task<TModel> AddAsync(TModel? model, CancellationToken cancellationToken = default)
    {
        await Set.AddAsync(model, cancellationToken);
        await SaveAsync(cancellationToken);
        return model;
    }

    public virtual async Task AddAsync(IEnumerable<TModel?> model, CancellationToken cancellationToken = default)
    {
        await Set.AddRangeAsync(model, cancellationToken);
        await SaveAsync(cancellationToken);
    }

    public async ValueTask<TModel?> GetAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await BaseQuery.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public virtual async ValueTask<TProject?> GetAsync<TProject>(TKey id, CancellationToken cancellationToken = default) where TProject : class
    {
        return await BaseQuery.Where(x => x.Id.Equals(id)).ProjectTo<TProject>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async ValueTask<TModel?> GetAsync(Expression<Func<TModel?, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await BaseQuery.FirstOrDefaultAsync(filter, cancellationToken);
    }

    public virtual async ValueTask<TProject?> GetAsync<TProject>(Expression<Func<TModel?, bool>> filter, CancellationToken cancellationToken = default) where TProject : class
    {
        return await BaseQuery.Where(filter).ProjectTo<TProject>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async ValueTask<TProject> GetAsync<TProject>(TKey id, Expression<Func<TModel, TProject>> projection, CancellationToken cancellationToken = default) where TProject : class
    {
        return await BaseQuery.Where(x => x.Id.Equals(id)).Select(projection).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async ValueTask<TProject?> GetAsync<TProject>(Expression<Func<TModel?, bool>> filter, Expression<Func<TModel, TProject>> projection, CancellationToken cancellationToken = default) where TProject : class
    {
        return await BaseQuery.Where(filter).ProjectTo<TProject>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async ValueTask<TKey?> GetIdAsync(Expression<Func<TModel?, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await BaseQuery.Where(filter).Select(x => x.Id).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async ValueTask<bool> HasAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await BaseQuery.AnyAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public virtual async ValueTask<bool> HasAsync(Expression<Func<TModel?, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await BaseQuery.AnyAsync(filter, cancellationToken);
    }

    public virtual async ValueTask<IEnumerable<TModel?>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await BaseQuery.ToArrayAsync(cancellationToken);
    }

    public virtual async ValueTask<IEnumerable<TProject>> ListAsync<TProject>(CancellationToken cancellationToken = default) where TProject : class
    {
        return await BaseQuery.ProjectTo<TProject>(_mapper.ConfigurationProvider).ToArrayAsync(cancellationToken);
    }

    public virtual async ValueTask<ICollection<TModel?>> ListAsync(Expression<Func<TModel?, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await BaseQuery.Where(expression).ToArrayAsync(cancellationToken);
    }

    public virtual async ValueTask<ICollection<TProject>> ListAsync<TProject>(Expression<Func<TModel?, bool>> expression, CancellationToken cancellationToken = default) where TProject : class
    {
        return await BaseQuery.Where(expression).ProjectTo<TProject>(_mapper.ConfigurationProvider).ToArrayAsync(cancellationToken);
    }

    public virtual async ValueTask<IEnumerable<TProject>> ListAsync<TProject>(Expression<Func<TModel, TProject>> projection, CancellationToken cancellationToken = default) where TProject : class
    {
        return await BaseQuery.Select(projection).ToArrayAsync(cancellationToken);
    }

    public virtual async ValueTask<IEnumerable<TProject>> ListAsync<TProject>(Expression<Func<TModel?, bool>> expression, Expression<Func<TModel, TProject>> projection, CancellationToken cancellationToken = default) where TProject : class
    {
        return await BaseQuery.Where(expression).Select(projection).ToArrayAsync(cancellationToken);
    }

    public async Task RemoveAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var model = await GetAsync(id, cancellationToken);
        Set.Remove(model);
        await SaveAsync(cancellationToken);
    }

    public async Task RemoveAsync(TModel? model, CancellationToken cancellationToken = default)
    {
        Set.Remove(model);
        await SaveAsync(cancellationToken);
    }

    public async Task RemoveRangeAsync(TModel[] models, CancellationToken cancellationToken = default)
    {
        Set.RemoveRange(models);
        await SaveAsync(cancellationToken);
    }

    public async Task UpdateAsync(TModel? model, CancellationToken cancellationToken = default)
    {
        Set.Update(model);
        await SaveAsync(cancellationToken);
    }

    public async Task UpdateRangeAsync(IEnumerable<TModel?> items, CancellationToken cancellationToken = default)
    {
        Set.UpdateRange(items);
        await SaveAsync(cancellationToken); 
    }

    public async ValueTask<int> SaveAsync(CancellationToken cancellationToken = default)
    {
        return await Db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<TModel?,bool>> expression,CancellationToken cancellationToken = default)
    {
        return await Set.Where(expression).AnyAsync(cancellationToken);
    }

    public virtual async Task<int> CountAsync(Expression<Func<TModel?, bool>> expression,CancellationToken cancellationToken = default)
    {
        return await Set.Where(expression).CountAsync(cancellationToken);
    }
}

public abstract class EFBaseRepository<TDbContext, Tmodel>: EFBaseRepository<TDbContext, Tmodel, int>,
    IEFBaseRepository<Tmodel> where Tmodel: class, IBaseEntity where TDbContext: DbContext
{
    protected EFBaseRepository(TDbContext db,IMapper mapper): base(db, mapper) 
    {
    }
}
