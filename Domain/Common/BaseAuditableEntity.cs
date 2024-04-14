namespace Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTime Created { get; init; }
    public DateTime? Updated { get; set; }
}
