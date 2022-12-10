using System.Dynamic;

namespace Blazor.Shared.Abstractions;

public interface IDataShaper<TEntity> where TEntity : class
{
    IEnumerable<ExpandoObject> ShapeData(IEnumerable<TEntity> entities, string fieldsString); 
    ExpandoObject ShapeData(TEntity entity, string fieldsString);
}