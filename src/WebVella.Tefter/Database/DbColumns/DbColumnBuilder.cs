namespace WebVella.Tefter.Database;

public abstract class DbColumnBuilder
{
    protected Guid _id = Guid.Empty;
    protected Guid? _applicationId = null;
    protected Guid? _dataProviderId = null;
    protected string _name = string.Empty;
    protected object _defaultValue = null;
    protected bool _isNullable = true;

    internal DbColumnBuilder() { }
    internal abstract DbColumn Build();
}