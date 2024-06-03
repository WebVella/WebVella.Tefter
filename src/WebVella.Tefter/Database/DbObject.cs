namespace WebVella.Tefter.Database;

public abstract class DbObject
{
    public virtual string Name { get; set; }

    public override string ToString()
    {
        return Name;
    }
}
