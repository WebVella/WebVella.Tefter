namespace WebVella.Tefter.UseCases.Space;
public partial class SpaceUseCase
{
	internal TucSpaceData ViewData = null;
	internal TucDataProvider DataProvider = null;


	internal Task InitSpaceViewManage()
	{
		AllDataProviders = GetDataProviderList();//should be in memory already
		return Task.CompletedTask;
	}

	internal Task<List<TucSpaceViewColumn>> InitSpaceViewManageAfterRender(TfAppState state)
	{
		if (state.SpaceView.SpaceDataId is not null)
			ViewData = state.SpaceDataList.FirstOrDefault(x => x.Id == state.SpaceView.SpaceDataId);
		if (ViewData is not null)
			DataProvider = AllDataProviders.FirstOrDefault(x => x.Id == ViewData.DataProviderId);

		return Task.FromResult(GetViewColumns(state.SpaceView.Id));
	}
}