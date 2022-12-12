namespace Blazor.Shared.Entities.LinkModels;

public sealed class LinkCollectionWrapper<TEntity> : LinkResourceBase
{
    public List<TEntity> Value { get; set; } = new();

    public LinkCollectionWrapper()
    {
    }

    public LinkCollectionWrapper(List<TEntity> value)
    {
        Value = value;
    }
}