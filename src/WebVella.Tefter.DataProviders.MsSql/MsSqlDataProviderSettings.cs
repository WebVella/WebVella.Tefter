namespace WebVella.Tefter.DataProviders.MsSql;

public class MsSqlDataProviderSettings
{
	[Required]
	public string ConnectionString { get; set; }

	[Required]
	public string SqlQuery { get; set; }
}
