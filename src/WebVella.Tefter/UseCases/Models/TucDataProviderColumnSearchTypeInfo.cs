namespace WebVella.Tefter.UseCases.Models;
public record TucDataProviderColumnSearchTypeInfo
{
	public int TypeValue { get; init; }
	public string Name { get; init; }

	public TucDataProviderColumnSearchTypeInfo() { }
	public TucDataProviderColumnSearchTypeInfo(TfDataProviderColumnSearchType model)
	{
		TypeValue = (int)model;
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
	public TfDataProviderColumnSearchType ToModel()
	{
		return (TfDataProviderColumnSearchType)TypeValue;
	}
}
