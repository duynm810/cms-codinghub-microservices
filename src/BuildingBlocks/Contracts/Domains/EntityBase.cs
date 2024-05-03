using Contracts.Domains.Interfaces;

namespace Contracts.Domains;

public class EntityBase<TKey> : IEntityBase<TKey>
{
    public required TKey Id { get; set; }
}