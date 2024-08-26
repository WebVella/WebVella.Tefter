namespace WebVella.Tefter.UseCases.Space;
public partial class SpaceUseCase
{
	internal List<TucDataProvider> AllDataProviders { get; set; } = new();
	internal Task InitSpaceDataManage()
	{
		AllDataProviders = GetDataProviderList();
		return Task.CompletedTask;
	}

}