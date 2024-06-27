namespace WebVella.Tefter.Database;

public abstract record DatabaseColumn : DatabaseObject
{
    #region <=== Meta ===>

    private DatabaseColumnMeta _meta = new();
    public Guid Id { get { return _meta.Id; } init { _meta.Id = value; } }
	public DatabaseColumnType Type { get { return _meta.ColumnType; } init { _meta.ColumnType = value; } }
	internal DateTime LastCommited { get { return _meta.LastCommited; } set { _meta.LastCommited= value; } }

    #endregion

    public virtual object DefaultValue { get; set; } = null;
    public virtual bool IsNullable { get; set; } = false;
    internal virtual string DatabaseColumnType { get; set; }

    internal string GetMetaJson(DateTime lastCommited)
    {
        var meta = _meta with { LastCommited = lastCommited };
        return JsonSerializer.Serialize(meta);
    }

    public override string ToString()
    {
        return Name;
    }
}
