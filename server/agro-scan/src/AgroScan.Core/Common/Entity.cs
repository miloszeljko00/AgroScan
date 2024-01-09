namespace AgroScan.Core.Common;
public abstract class Entity(Guid id)
{
    public Guid Id { get; set; } = id;
}
