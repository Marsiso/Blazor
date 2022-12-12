using System.Dynamic;
using System.Reflection;
using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.Models;

namespace Blazor.Shared.Implementations.DataShaping;

public sealed class DataShaper<TEntity> : IDataShaper<TEntity> where TEntity : class
{
    public PropertyInfo[] Properties { get; set; }

    public DataShaper()
    {
        Properties = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);
    }

    public IEnumerable<ShapedEntity> ShapeData(IEnumerable<TEntity> entities, string fieldsString)
    {
        var requiredProperties = GetRequiredProperties(fieldsString);
        
        return FetchData(entities, requiredProperties);
    }

    public ShapedEntity ShapeData(TEntity entity, string fieldsString)
    {
        var requiredProperties = GetRequiredProperties(fieldsString); 
        
        return FetchDataForEntity(entity, requiredProperties);
    }
    
    private IEnumerable<PropertyInfo> GetRequiredProperties(string fieldsString)
    {
        var requiredProperties = new List<PropertyInfo>();
        if (!string.IsNullOrWhiteSpace(fieldsString))
        {
            var fields = fieldsString.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var field in fields)
            {
                var property = Properties.FirstOrDefault(pi =>
                    pi.Name.Equals(field.Trim(), StringComparison.InvariantCultureIgnoreCase));
                if (property == null)
                {
                    continue;
                }
                
                requiredProperties.Add(property);
            }
        }
        else
        {
            requiredProperties = Properties.ToList();
        }

        return requiredProperties;
    }
    
    private IEnumerable<ShapedEntity> FetchData(IEnumerable<TEntity> entities, IEnumerable<PropertyInfo> requiredProperties)
    {
        var shapedData = new List<ShapedEntity>();
        foreach (var entity in entities)
        {
            var shapedObject = FetchDataForEntity(entity, requiredProperties);
            shapedData.Add(shapedObject);
        }

        return shapedData;
    }

    private ShapedEntity FetchDataForEntity(TEntity entity, IEnumerable<PropertyInfo> requiredProperties)
    {
        var shapedObject = new ShapedEntity();
        foreach (var property in requiredProperties)
        {
            var objectPropertyValue = property.GetValue(entity);
            shapedObject.Entity.TryAdd(property.Name, objectPropertyValue);
        }

        var objectProperty = entity.GetType().GetProperty("Id");
        shapedObject.Id = (int)objectProperty.GetValue(entity);

        return shapedObject;
    }
}