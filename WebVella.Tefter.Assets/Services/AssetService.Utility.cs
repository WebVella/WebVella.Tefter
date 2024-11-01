namespace WebVella.Tefter.Assets.Services;

internal partial class AssetService : IAssetService
{
	private static NpgsqlParameter CreateParameter(
		string name, 
		object value,
		DbType type)
	{
		NpgsqlParameter par = new NpgsqlParameter(name, type);
		if (value is null)
			par.Value = DBNull.Value;
		else
			par.Value = value;

		return par;
	}
}
