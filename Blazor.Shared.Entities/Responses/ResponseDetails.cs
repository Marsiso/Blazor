using System.Text.Json;

namespace Blazor.Shared.Entities.Responses;

public sealed class ResponseDetails
{
    public string Message { get; set; }
    public bool IsSuccess { get; set; }
    public bool IsResponse { get; set; }
    public override string ToString() => JsonSerializer.Serialize(this);
}
