namespace WebVella.Tefter.UseCases.UserAdmin;
public partial class UserAdminUseCase
{
	internal string ActionsMenu { get; set; } = "details";

	internal async Task InitForDetailsActionsAsync()
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
