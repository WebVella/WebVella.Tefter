namespace WebVella.Tefter.Database.Dbo;
internal record DboModelMeta
{
    public bool IsSqlModel { get; internal set; }
    public Type ModelType { get; internal set; }
    public string TableName { get; internal set; }
    public bool UseCache { get; internal set; }
    public MemoryCacheOptions CacheOptions { get; internal set; }
    public List<DboModelPropertyMeta> Properties { get; internal set; } = new List<DboModelPropertyMeta>();

    public string GetSql {get; internal set; }
    public string GetRecordSql { get; internal set; }
    public string InsertRecordSql { get; internal set; }
    public string UpdateRecordSql { get; internal set; }
    public string DeleteRecordSql { get; internal set; }
    public string DeleteAllRecordSql { get; internal set; }
    public string AnyExistsSql { get; internal set; }

    public override string ToString()
    {
        return $"Table={TableName}  Type={ModelType}  PropsCount={Properties.Count} UseCache:{UseCache}";
    }
}
