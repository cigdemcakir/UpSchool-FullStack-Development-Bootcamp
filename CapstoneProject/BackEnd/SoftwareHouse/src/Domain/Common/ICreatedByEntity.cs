namespace Domain.Common;

public interface ICreatedByEntity 
{
    DateTimeOffset CreatedOn { get; set; }
}