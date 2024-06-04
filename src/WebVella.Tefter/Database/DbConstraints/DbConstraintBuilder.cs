namespace WebVella.Tefter.Database;

public abstract class DbConstraintBuilder
{
    protected string _name = string.Empty;
    protected List<string> _columns = new List<string>();

    internal abstract DbConstraint Build();
}