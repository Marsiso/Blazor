namespace Blazor.Shared.Abstractions;

public interface IEntity<TEntity> where TEntity : class
{
    int Id { get; set; }
}
