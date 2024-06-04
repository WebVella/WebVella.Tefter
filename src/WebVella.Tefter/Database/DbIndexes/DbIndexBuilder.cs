namespace WebVella.Tefter.Database;

public abstract class DbIndexBuilder
{
    protected string _name = string.Empty;
    protected List<string> _columns = new List<string>();

    internal abstract DbIndex Build();
}