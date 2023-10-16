using Ardalis.GuardClauses;

namespace PortfolioService.Core.Domain.Core.Primitives;

// public abstract class Entity: IEquatable<Entity>
// {
//
//     /// <summary>
//     /// Initializes a new instance of the <see cref="Entity"/> class.
//     /// </summary>
//     /// <param name="id">The entity identifier.</param>
//     protected Entity(Guid id)
//         : this()
//     {
//         ArgumentNullException.ThrowIfNullOrEmpty(id,nameof(id));
//     //    Ensure.NotEmpty(id, "The identifier is required.", nameof(id));
//
//         Id = id;
//     }
//
//
//     public  string Id {  get; protected set; }
//     public DateTime CreatedAt => DateTime.Now;
// }

public abstract class Entity : IEquatable<Entity>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Entity" /> class.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    protected Entity(string id)
        : this()
    {
        Guard.Against.Null(id);
        Id = id;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Entity" /> class.
    /// </summary>
    /// <remarks>
    ///     Required by EF Core.
    /// </remarks>
    protected Entity()
    {
    }

    /// <summary>
    ///     Gets or sets the entity identifier.
    /// </summary>
    public string Id { get; }

    /// <inheritdoc />
    public bool Equals(Entity other)
    {
        if (other is null) return false;

        return ReferenceEquals(this, other) || Id == other.Id;
    }

    public static bool operator ==(Entity a, Entity b)
    {
        if (a is null && b is null) return true;

        if (a is null || b is null) return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity a, Entity b)
    {
        return !(a == b);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        if (obj is null) return false;

        if (ReferenceEquals(this, obj)) return true;

        if (obj.GetType() != GetType()) return false;

        if (!(obj is Entity other)) return false;

        if (Id == string.Empty || other.Id == string.Empty) return false;

        return Id == other.Id;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Id.GetHashCode() * 41;
    }
}
