namespace WebVella.Tefter.Talk.Services;

internal class TalkUtility
{
	public static NpgsqlParameter CreateParameter( string name, object? value, DbType type )
	{
		NpgsqlParameter par = new NpgsqlParameter(name, type);
		if (value is null)
			par.Value = DBNull.Value;
		else
			par.Value = null;

		return par;
	}
}
