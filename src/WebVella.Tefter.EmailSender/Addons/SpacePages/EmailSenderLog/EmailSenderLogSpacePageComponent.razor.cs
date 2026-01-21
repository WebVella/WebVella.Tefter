using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.EmailSender.Addons;
public partial class EmailSenderLogSpacePageComponent : TucBaseSpacePageComponent, IDisposable
{
	public static readonly string Id = "033DC3C2-8460-4029-9EFC-AC19FD46BD7F";
	public static readonly string Name = "Emails Log";
	public static readonly string Description = "lists schedule emails details";
	public static readonly string FluentIconName = "Mail";

	#region << Injects >>
	[Inject] protected IEmailService EmailService { get; set; }
	[Inject] protected IJSRuntime JSRuntime { get; set; } = null!;
	[Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
	[Inject] protected NavigationManager Navigator { get; set; } = null!;
	[Inject] protected IDialogService DialogService { get; set; } = null!;
	#endregion

	#region << Base Overrides >>
	public override Guid AddonId { get; init; } = new Guid(Id);
	public override string AddonName { get; init; } = Name;
	public override string AddonDescription { get; init; } = Description;
	public override string AddonFluentIconName { get; init; } = FluentIconName;
	[Parameter] public override TfSpacePageAddonContext Context { get; set; }

	public override string GetOptions() => JsonSerializer.Serialize(_options);
	public override List<ValidationError> ValidateOptions()
	{
		ValidationErrors.Clear();
		if (_options.RelatedIdValueType == EmailSenderLogRelatedIdValueType.AnyRelatedId
		    || _options.RelatedIdValueType == EmailSenderLogRelatedIdValueType.AnyRelatedId)
		{
			if (!Guid.TryParse(_options.CustomIdValue, out _))
			{
				ValidationErrors.Add(new ValidationError(nameof(_options.CustomIdValue), "invalid GUID"));	
			}
		}		
		return ValidationErrors;
	}

	public override async Task<string> OnPageCreated(IServiceProvider serviceProvider, TfSpacePageAddonContext context)
	{
		await base.OnPageCreated(serviceProvider, context);
		if (String.IsNullOrWhiteSpace(context.ComponentOptionsJson)) throw new Exception("EmailSenderLog error: ComponentOptionsJson is null");
		var jsonOptions = JsonSerializer.Deserialize<EmailSenderLogSpacePageComponentOptions>(context.ComponentOptionsJson);
		if (jsonOptions is null) throw new Exception("EmailSenderLog error: options cannot be deserialized");

		return context.ComponentOptionsJson;
	}
	public override List<TfScreenRegionTab> GetManagementTabs()
	{
		var tabs = new List<TfScreenRegionTab>()
		{
			new TfScreenRegionTab("filter", "Emails Filter", "Filter"),
		};
		return tabs;
	}	
	#endregion

	#region << Private properties >>
	//OptionsMode
	private string optionsJson = "{}";
	private EmailSenderLogSpacePageComponentOptions _options { get; set; } = new();

	//Read mode
	private Guid _spacePageId;
	private bool _isLoaded = false;
	private string _uriInitialized = string.Empty;

	#endregion


	#region << Render Lifecycle >>
	public void Dispose()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		_isLoaded = true;
	}
	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Context.ComponentOptionsJson != optionsJson)
		{
			optionsJson = Context.ComponentOptionsJson;
			_options = JsonSerializer.Deserialize<EmailSenderLogSpacePageComponentOptions>(optionsJson);
			//When cannot node has json from another page type
			if (_options is null) _options = new();
		}
	}

	#endregion

	#region << Private methods >>
	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (_uriInitialized != args.Location)
				await _init(navState: TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			_spacePageId = navState.SpacePageId ?? Guid.Empty;
			_options = null;
			if (!String.IsNullOrWhiteSpace(Context.ComponentOptionsJson))
			{
				optionsJson = Context.ComponentOptionsJson;
				_options = JsonSerializer.Deserialize<EmailSenderLogSpacePageComponentOptions>(optionsJson);
			}
		}
		finally
		{
			_uriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _editFolder()
	{
		var dialog = await DialogService.ShowDialogAsync<EmailSenderLogPageManageDialog>(
			new EmailSenderLogPageManageDialogContext() { Options = _options, SpacePageId = _spacePageId },
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		_ = await dialog.Result;
	}	
	#endregion
}

public record EmailSenderLogSpacePageComponentOptions
{
	[JsonPropertyName("RelatedIdValueType")]
	public EmailSenderLogRelatedIdValueType RelatedIdValueType { get; set; } = EmailSenderLogRelatedIdValueType.SpaceId;

	[JsonPropertyName("CustomIdValue")]
	public string CustomIdValue { get; set; } = "";
}
