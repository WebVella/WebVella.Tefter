namespace WebVella.Tefter.DataProviders.MsSql;

internal class MsSqlDataProviderSettings
{
	[Required]
	public string ConnectionString { get; set; }

	[Required]
	public string SqlQuery { get; set; }
}
