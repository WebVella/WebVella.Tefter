namespace WebVella.Tefter.Database;

public class DbObjectMeta
{
    public Guid Id { get; set; }
    public DbObjectType Type { get; set; } = DbObjectType.System;

    internal DbObjectMeta() : this(Guid.Empty, DbObjectType.System)
    {
    }

    internal DbObjectMeta(Guid id, DbObjectType type)
    {
        Id = id;
        Type = type;
    }
}
