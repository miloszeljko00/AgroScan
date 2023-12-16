namespace AgroScan.Core.Common;
public abstract class Entity<T>(T id)
{
    public T Id { get; set; } = id;
}
