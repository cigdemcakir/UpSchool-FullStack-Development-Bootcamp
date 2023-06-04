namespace Domain.Common;

public class EntityBase<TKey>:IEntityBase<TKey>,ICreatedByEntity,IModifiedByEntity,IDeletedByEntity
{
    public TKey Id { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset? ModifiedOn { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public bool IsDeleted { get; set; }
}