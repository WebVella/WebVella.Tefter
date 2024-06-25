namespace WebVella.Tefter.Web.Components.DataProviderManageDialog;
public partial class TfDataProviderManageDialog : TfFormBaseComponent, IDialogContentComponent<TfDataProvider>
{
	[Parameter]
	public TfDataProvider Content { get; set; }

	[CascadingParameter]
	public FluentDialog Dialog { get; set; }

	private bool _isBusy = true;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private Icon _icon;
	private TfDataProviderDialogModel _form = new();
	private List<ITfDataProviderType> _allTypes = new();
	private DynamicComponent typeSettingsComponent;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (Content is null)
		{
			_title = LC["Create data provider"];
			_icon = new Icons.Regular.Size20.Add();
		}
		else
		{
			_title = LC["Manage data provider"];
			_icon = new Icons.Regular.Size20.Edit();
		}
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			await _loadDataAsync();
			_isBusy = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _loadDataAsync()
	{
		_isBusy = true;
		await InvokeAsync(StateHasChanged);
		try
		{
			//Init type options
			var typesResult = DataProviderManager.GetProviderTypes();
			if (!typesResult.IsSuccess) throw new Exception("Cannot load data provider types");
			_allTypes = typesResult.Value.ToList();
			if (!_allTypes.Any()) throw new Exception("No Data provider types found in application");
			//Setup form
			if (Content is null)
			{
				_form = new TfDataProviderDialogModel()
				{
					ProviderType = _allTypes[0]
				};
			}
			else
			{
				_form = new TfDataProviderDialogModel()
				{
					Id = Content.Id,
					Name = Content.Name,
					ProviderType = Content.ProviderType,
					CompositeKeyPrefix = Content.CompositeKeyPrefix,
					SettingsJson =Content.SettingsJson
				};
			}
			base.InitForm(_form);
		}
		catch (Exception ex)
		{
			_error = ProcessException(ex);
		}
		finally
		{


		}
	}

	private async Task _save()
	{
		if (_isSubmitting) return;
		try
		{
			//Workaround to wait for the form to be bound 
			//on enter click without blur
			await Task.Delay(10);


			var isValid = EditContext.Validate();
			if (!isValid) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);

			//UserBuilder userBuilder;
			//if (Content is null)
			//{
			//	userBuilder = IdentityManager.CreateUserBuilder(null);
			//	userBuilder
			//		.WithEmail(_form.Email)
			//		.WithFirstName(_form.FirstName)
			//		.WithLastName(_form.LastName)
			//		.WithPassword(_form.Password)
			//		.Enabled(_form.Enabled)
			//		.CreatedOn(DateTime.Now)
			//		.WithThemeMode(_form.ThemeMode)
			//		.WithThemeColor(_form.ThemeColor)
			//		.WithOpenSidebar(true)
			//		.WithCultureCode(_form.Culture.CultureInfo.Name)
			//		.WithRoles(_form.Roles.ToArray());
			//}
			//else
			//{
			//	userBuilder = IdentityManager.CreateUserBuilder(Content);
			//	userBuilder
			//		.WithEmail(_form.Email)
			//		.WithFirstName(_form.FirstName)
			//		.WithLastName(_form.LastName)
			//		.Enabled(_form.Enabled)
			//		.WithThemeMode(_form.ThemeMode)
			//		.WithThemeColor(_form.ThemeColor)
			//		.WithCultureCode(_form.Culture.CultureInfo.Name)
			//		.WithRoles(_form.Roles.ToArray());
			//}

			//var user = userBuilder.Build();
			//var result = await IdentityManager.SaveUserAsync(user);
			//ProcessFormSubmitResponse(result);
			//if (result.IsSuccess)
			//{
			//	await Dialog.CloseAsync(result.Value);
			//}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isSubmitting = false;
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}


}

public class TfDataProviderDialogModel
{
	[Required]
	public Guid Id { get; set; }

	[Required]
	public string Name { get; set; }

	public string CompositeKeyPrefix { get; set; }

	[Required]
	public ITfDataProviderType ProviderType { get; set; }

	public string SettingsJson { get; set; } = null;

}