namespace WebVella.Tefter.UseCases.DataProviderAdmin;
public partial class DataProviderAdminUseCase
{
	internal string ActionsMenu { get; set; } = "details";
	internal async Task InitForDetailsActions()
	{
		await InitActionsMenu(null);
	}



	internal Task InitActionsMenu(string url)
	{
		var urlData = _navigationManager.GetUrlData(url);
		if (urlData.SegmentsByIndexDict.ContainsKey(3))
			ActionsMenu = urlData.SegmentsByIndexDict[3];
		else
			ActionsMenu = "details";

		return Task.CompletedTask;
	}
}
