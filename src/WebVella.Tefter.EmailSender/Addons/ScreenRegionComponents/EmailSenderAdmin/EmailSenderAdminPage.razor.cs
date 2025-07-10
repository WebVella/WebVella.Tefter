namespace WebVella.Tefter.EmailSender.Addons;

public partial class EmailSenderAdminPage : TfBaseComponent, ITfScreenRegionComponent<TfAdminPageScreenRegionContext>
{
	public const string ID = "1f6e544e-6a53-4fa1-98ef-9c51a569c2b5";
	public const string NAME = "Email Sender";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "Mail";
	public const int POSITION_RANK = 990;
	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){
			new TfScreenRegionScope(null,new Guid(ID))
		};
	[Parameter]
	public TfAdminPageScreenRegionContext RegionContext { get; init; }

	//public Task OnAppStateInit(IServiceProvider serviceProvider, TucUser currentUser,
	//	TfAppState newAppState,
	//	TfAppState oldAppState, TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	//{
	//	var mailService = serviceProvider.GetRequiredService<IEmailService>();
	//	try
	//	{
	//		var emails = mailService.GetEmailMessages(newAppState.Route.Search, newAppState.Route.Page, newAppState.Route.PageSize);
	//		newAuxDataState.Data[EmailSenderConstants.APP_EMAIL_LIST_DATA_KEY] = emails;
	//	}
	//	catch
	//	{
	//		newAuxDataState.Data[EmailSenderConstants.APP_EMAIL_LIST_DATA_KEY] = new List<EmailMessage>();
	//	}

	//	return Task.CompletedTask;
	//}

	private async Task _createTestHandler()
	{
		var dialog = await DialogService.ShowDialogAsync<SendTestEmailDialog>(
		new EmailMessage(),
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			//if (TfAppState.Value.Route.Page == 1)
			//{
			//	var items = (List<EmailMessage>)TfAuxDataState.Value.Data[EmailSenderConstants.APP_EMAIL_LIST_DATA_KEY];
			//	items.Insert(0, (EmailMessage)result.Data);
			//	Dispatcher.Dispatch(new SetAuxDataStateByKeyAction(component: this,
			//		dictKey: EmailSenderConstants.APP_EMAIL_LIST_DATA_KEY,
			//		dictData: items));
			//}
		}
	}

	private async Task _searchValueChanged(string search)
	{
		var queryDict = new Dictionary<string, object>{
			{TfConstants.SearchQueryName,search}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
		await InvokeAsync(StateHasChanged);
	}

	private async Task _goFirstPage()
	{
		//if (TfAppState.Value.Route.Page == 1) return;
		//var queryDict = new Dictionary<string, object>{
		//	{TfConstants.PageQueryName,1}
		//};
		//await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goPreviousPage()
	{
		//var page = TfAppState.Value.Route.Page - 1;
		//if (page < 1) page = 1;
		//if (TfAppState.Value.Route.Page == page) return;
		//var queryDict = new Dictionary<string, object>{
		//	{TfConstants.PageQueryName, page}
		//};
		//await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goNextPage()
	{
		//var page = TfAppState.Value.Route.Page + 1;
		//if (page < 1) page = 1;
		//if (TfAppState.Value.Route.Page == page) return;

		//var queryDict = new Dictionary<string, object>{
		//	{TfConstants.PageQueryName,page}
		//};
		//await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goLastPage()
	{
		//if (TfAppState.Value.Route.Page == -1) return;
		//var queryDict = new Dictionary<string, object>{
		//	{TfConstants.PageQueryName, -1}
		//};
		//await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goOnPage(int page)
	{
		//if (page < 1 && page != -1) page = 1;
		//if (TfAppState.Value.Route.Page == page) return;
		//var queryDict = new Dictionary<string, object>{
		//	{TfConstants.PageQueryName, page}
		//};
		//await Navigator.ApplyChangeToUrlQuery(queryDict);
	}


}