namespace WebVella.Tefter.Web.Pages;
public partial class AdminSharedColumnsPage : TfBasePage
{
	[Parameter] public Guid SharedColumnId { get; set; }
	[Parameter] public string Path { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (SharedColumnId == Guid.Empty && TfAppState.Value.AdminSharedColumns is not null
			&& TfAppState.Value.AdminSharedColumns.Count > 0)
		{
			Navigator.NavigateTo(string.Format(TfConstants.AdminSharedColumnDetailsPageUrl, TfAppState.Value.AdminSharedColumns[0].Id));
		}
	}

}