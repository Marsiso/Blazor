using System.Reflection;
using System.Text;

namespace Blazor.Shared.Implementations.Extensions.Utility;

public static class OrderQueryBuilder
{
    public static string CreateOrderQuery<TEntity>(string orderByQueryString)
    {
        var orderParams = orderByQueryString.Trim().Split(',');
        var propertyInfos = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var orderQueryBuilder = new StringBuilder();
        
        foreach (var param in orderParams)
        {
            if (string.IsNullOrWhiteSpace(param))
            {
                continue;
            }
            
            var propertyFromQueryName = param.Split(" ")[0];
            var objectProperty = propertyInfos.FirstOrDefault(pi =>
                pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));
            if (objectProperty == null)
            {
                continue;
            }
            
            var direction = param.EndsWith(" desc") ? "descending" : "ascending";
            orderQueryBuilder.Append($"{objectProperty.Name} {direction}, "); 
        } 
        
        var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
        return orderQuery; 
    } 
}