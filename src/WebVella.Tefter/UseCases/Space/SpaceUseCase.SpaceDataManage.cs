namespace WebVella.Tefter.UseCases.Space;
public partial class SpaceUseCase
{
	internal Task InitSpaceDataManage()
	{
		AllDataProviders = GetDataProviderList();

		return Task.CompletedTask;
	}

}