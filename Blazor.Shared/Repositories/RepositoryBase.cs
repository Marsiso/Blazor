using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Blazor.Shared.Implementations.Repositories;

public abstract class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
{
    protected SqlContext Context { get; set; }

    protected RepositoryBase(SqlContext context)
    {
        Context = context;
    }


    public IQueryable<TEntity> FindAll(bool trackChanges) =>
        !trackChanges
            ? Context
                .Set<TEntity>()
                .AsNoTracking()
            : Context
                .Set<TEntity>()
                .AsTracking();

    public IQueryable<TEntity> FindByCondition(Expression<Func<TEntity, bool>> expression, bool trackChanges) =>
        !trackChanges
            ? Context
                .Set<TEntity>()
                .Where(expression)
                .AsNoTracking()
            : Context
                .Set<TEntity>()
                .Where(expression)
                .AsTracking();

    public void Create(TEntity entity) => Context.Set<TEntity>().Add(entity);

    public void Delete(TEntity entity) => Context.Set<TEntity>().Remove(entity);

    public void Update(TEntity entity) => Context.Set<TEntity>().Update(entity);
}
