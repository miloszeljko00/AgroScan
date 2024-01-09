
namespace AgroScan.Core.Common;
public abstract class AuditableEntity(Guid id): Entity(id) { 
 
    public DateTimeOffset Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset LastModified { get; set; }

    public string? LastModifiedBy { get; set; }
}