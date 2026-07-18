namespace Skvia.Attendance.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; private set; }

    protected Entity(Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
    }

    private Entity() { }
}
