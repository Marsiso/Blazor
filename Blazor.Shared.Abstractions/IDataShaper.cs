using System.Dynamic;
using Blazor.Shared.Entities.Models;

namespace Blazor.Shared.Abstractions;

public interface IDataShaper<TEntity> where TEntity : class
{
    IEnumerable<ShapedEntity> ShapeData(IEnumerable<TEntity> entities, string fieldsString); 
    ShapedEntity ShapeData(TEntity entity, string fieldsString);
}