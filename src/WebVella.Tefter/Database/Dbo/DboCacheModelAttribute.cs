namespace WebVella.Tefter.Database.Dbo;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
internal class DboCacheModelAttribute : Attribute
{
    static MemoryCacheOptions defaultOptions = new MemoryCacheOptions{ ExpirationScanFrequency = TimeSpan.FromHours(1) };
    public MemoryCacheOptions Options { get; }

    public DboCacheModelAttribute()
    {
        Options = defaultOptions;
    }

    public DboCacheModelAttribute(MemoryCacheOptions options)
    {
        Options = options;
        if (options is null)
            Options = defaultOptions;

    }
}
