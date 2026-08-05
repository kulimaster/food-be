namespace Food.Domain.Common;

public abstract class Entity
{
    public long Id { get; protected set; }

    public override bool Equals(object? obj) =>
        obj is Entity other && other.GetType() == GetType() && Id != 0 && Id == other.Id;

    public override int GetHashCode() => (GetType(), Id).GetHashCode();
}
