namespace Blazor.Shared.Entities.Models;

public sealed class ShapedEntity
{
    public int Id { get; set; }

    public Entity Entity { get; set; }

    public ShapedEntity()
    {
        Entity = new Entity();
    }
}