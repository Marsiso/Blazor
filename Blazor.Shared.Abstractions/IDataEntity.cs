namespace Blazor.Shared.Abstractions;

public interface IDataEntity<TEntity> where TEntity : class, IEntity<TEntity>
{
}
