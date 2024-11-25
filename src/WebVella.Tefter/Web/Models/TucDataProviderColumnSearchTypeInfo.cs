namespace WebVella.Tefter.Web.Models;
public record TucDataProviderColumnSearchTypeInfo
{
	public TfDataProviderColumnSearchType TypeValue { get; init; }
	public string Name { get; init; }

	public TucDataProviderColumnSearchTypeInfo() { }
	public TucDataProviderColumnSearchTypeInfo(TfDataProviderColumnSearchType model)
	{
		switch (model)
		{
			case TfDataProviderColumnSearchType.Equals:
			case TfDataProviderColumnSearchType.Comparison:
			case TfDataProviderColumnSearchType.Contains:
				{
					Name = model.ToString();
				}
				break;
			default:
				throw new Exception($"TfDataProviderColumnSearchType: '{model}' not supported by use case");
		}
	}
}
