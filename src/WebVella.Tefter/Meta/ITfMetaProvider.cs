namespace WebVella.Tefter;

public partial interface ITfMetaProvider
{
	public Type GetTypeForFullClassName(
		string fullClassName );
}

public partial class TfMetaProvider : ITfMetaProvider
{
	private static Dictionary<string, Type> _typesMap = new Dictionary<string, Type>();

	public Type GetTypeForFullClassName(
		string fullClassName)
	{
		if(fullClassName is null)
		{
			return null;
		}

		if(!_typesMap.ContainsKey( fullClassName ) )
		{
			return null;
		}

		return _typesMap[fullClassName];
	}
}
