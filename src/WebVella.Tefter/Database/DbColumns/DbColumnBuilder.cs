namespace WebVella.Tefter.Database;

public abstract class DbColumnBuilder
{
    protected bool _isNew;
    protected string _name;
    protected DbTableBuilder _tableBuilder;
    protected object _defaultValue = null;
    protected bool _isNullable = true;
    
    internal string Name { get { return _name; } }

    internal DbColumnBuilder(string name, bool isNew, DbTableBuilder tableBuilder) 
    { 
        _name = name;
        _isNew = isNew;
        _tableBuilder = tableBuilder;
    }

    internal abstract DbColumn Build();
}