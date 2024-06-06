namespace WebVella.Tefter.Database;

public abstract class DbColumnBuilder
{
    protected DbTableBuilder _tableBuilder;

    protected Guid _id;
    protected string _name;
    protected object _defaultValue = null;
    protected bool _isNullable = true;
    protected bool _autoDefaultValue = false;

    internal string Name { get { return _name; } }

    internal DbColumnBuilder(string name, DbTableBuilder tableBuilder)
    {
        _id = Guid.NewGuid();
        _name = name;
        _tableBuilder = tableBuilder;
    }

    internal DbColumnBuilder(DbColumn column, DbTableBuilder tableBuilder)
    {
        _id = column.Id;
        _tableBuilder = tableBuilder;
        _name = column.Name;
        _isNullable = column.IsNullable;
        _defaultValue = column.DefaultValue;
        if (column.GetType().IsAssignableFrom(typeof(DbColumnWithAutoDefaultValue)))
        {
            _autoDefaultValue = ((DbColumnWithAutoDefaultValue)column).AutoDefaultValue;
        }
    }

    internal abstract DbColumn Build();
}