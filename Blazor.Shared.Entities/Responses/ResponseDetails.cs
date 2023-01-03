using System.Text.Json;
using Blazor.Shared.Entities.LinkModels;
using Blazor.Shared.Entities.Models;
using Blazor.Shared.Entities.RequestFeatures;

namespace Blazor.Shared.Entities.Responses;

public sealed class ResponseDetails<TEntity>
{
    public string Message { get; set; }
    public bool IsSuccess { get; set; }
    public bool IsResponse { get; set; }
    public MetaData MetaData { get; set; }
    public TEntity Content { get; set; }
    public override string ToString() => JsonSerializer.Serialize(this);
}
