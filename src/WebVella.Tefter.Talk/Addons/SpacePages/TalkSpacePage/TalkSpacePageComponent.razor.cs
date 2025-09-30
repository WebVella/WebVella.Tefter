using Microsoft.AspNetCore.Components.Authorization;
using WebVella.Tefter.Exceptions;
using WebVella.Tefter.UIServices;

namespace WebVella.Tefter.Talk.Addons;
public partial class TalkSpacePageComponent : TucBaseSpacePageComponent, IDisposable
{
	public const string ID = "6589259a-4f90-445d-8a62-d4b51bab3afd";
	public const string NAME = "Talk Page";
	public const string DESCRIPTION = "general discussion per page";
	public const string FLUENT_ICON_NAME = "ChatMultiple";

	#region << Injects >>
	[Inject] protected ITalkService TalkService { get; set; }
	[Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
	[Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
	[Inject] protected NavigationManager Navigator { get; set; } = default!;
	#endregion

	#region << Base Overrides >>
	public override Guid AddonId { get; init; } = new Guid(ID);
	public override string AddonName { get; init; } = NAME;
	public override string AddonDescription { get; init; } = DESCRIPTION;
	public override string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	[Parameter] public override TfSpacePageAddonContext Context { get; set; }

	public override string GetOptions() => JsonSerializer.Serialize(_options);
	public override List<ValidationError> ValidateOptions()
	{
		ValidationErrors.Clear();

		if (_options.ChannelId is null)
		{
			ValidationErrors.Add(new ValidationError(nameof(_options.ChannelId), "required"));
		}

		return ValidationErrors;
	}

	public override async Task<string> OnPageCreated(IServiceProvider serviceProvider, TfSpacePageAddonContext context)
	{
		await base.OnPageCreated(serviceProvider, context);
		if (String.IsNullOrWhiteSpace(context.ComponentOptionsJson)) throw new Exception("TalkPageComponent error: ComponentOptionsJson is null");
		var jsonOptions = JsonSerializer.Deserialize<TalkSpacePageComponentOptions>(context.ComponentOptionsJson);
		if (jsonOptions is null) throw new Exception("TalkPageComponent error: options cannot be deserialized");

		return context.ComponentOptionsJson;
	}

	#endregion

	#region << Private properties >>
	//OptionsMode
	private string optionsJson = "{}";
	private TalkSpacePageComponentOptions _options { get; set; } = new();
	private TalkChannel _optionsChannel { get; set; } = null;
	private List<TalkChannel> _channels { get; set; } = new();

	//Read mode
	private TfUser _currentUser = new();
	private TalkChannel _channel = null;
	private string _dataIdentityValue = null;
	private bool _isLoaded = false;
	private string _uriInitialized = string.Empty;

	#endregion


	#region << Render Lifecycle >>
	public void Dispose()
	{
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		_currentUser = TfAuthLayout.CurrentUser;
		await _init();
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
		_isLoaded = true;
	}
	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Context.ComponentOptionsJson != optionsJson)
		{
			optionsJson = Context.ComponentOptionsJson;
			_options = JsonSerializer.Deserialize<TalkSpacePageComponentOptions>(optionsJson);
			//When cannot node has json from another page type
			if (_options is null) _options = new();
			if (_options.ChannelId is null && _optionsChannel is not null) _options.ChannelId = _optionsChannel.Id;
		}
	}

	#endregion

	#region << Private methods >>
	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (_uriInitialized != args.Uri)
			await _init(navState: args);
	}

	private async Task _init(TfNavigationState? navState = null)
	{
		if (navState is null)
			navState = TfAuthLayout.NavigationState;
		try
		{
			_options = null;
			if (!String.IsNullOrWhiteSpace(Context.ComponentOptionsJson))
			{
				optionsJson = Context.ComponentOptionsJson;
				_options = JsonSerializer.Deserialize<TalkSpacePageComponentOptions>(optionsJson);
			}
			_dataIdentityValue = await _getDataIdentityValue();
			if (Context.Mode == TfComponentMode.Read)
			{
				_channel = null;
				if (_options?.ChannelId is not null)
				{
					_channel = TalkService.GetChannel(_options.ChannelId.Value);
				}
			}
			else
			{
				_channels = TalkService.GetChannels();
				_optionsChannel = null;
				if (_options?.ChannelId is not null)
				{
					_optionsChannel = _channels.FirstOrDefault(x => x.Id == _options.ChannelId);
				}
				if (_optionsChannel is null && _channels.Count > 0)
				{
					_optionsChannel = _channels[0];
					_options.ChannelId = _optionsChannel.Id;
				}
			}
		}
		finally
		{
			_uriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task<string> _getDataIdentityValue()
	{
		if (_options is null) return Guid.NewGuid().ToSha1();
		var navState = TfAuthLayout.NavigationState;
		switch (_options.DataIdentityValueType)
		{
			case TalkChannelDataIdentityValueType.SpacePageId:
				return (navState.SpacePageId ?? Guid.Empty).ToSha1();
			case TalkChannelDataIdentityValueType.SpaceId:
				return (navState.SpaceId ?? Guid.Empty).ToSha1();
			case TalkChannelDataIdentityValueType.CustomString:
				return String.IsNullOrWhiteSpace(_options.CustomDataIdentityValue) ? null : _options.CustomDataIdentityValue.ToSha1();
			default:
				throw new Exception("Shared Key Type not supported");
		}
	}

	private Task _optionsChannelChangeHandler(TalkChannel channel)
	{
		_optionsChannel = channel;
		_options.ChannelId = channel?.Id;
		return Task.CompletedTask;
	}

	#endregion
}

public class TalkSpacePageComponentOptions
{
	[JsonPropertyName("ChannelId")]
	public Guid? ChannelId { get; set; } = null;

	[JsonPropertyName("DataIdentityValueType")]
	public TalkChannelDataIdentityValueType DataIdentityValueType { get; set; } = TalkChannelDataIdentityValueType.SpacePageId;

	[JsonPropertyName("CustomDataIdentityValue")]
	public string CustomDataIdentityValue { get; set; } = "";
}

public class TalkSpacePageComponentContext
{
	[JsonPropertyName("TalkChannels")]
	public List<TalkChannel> TalkChannels { get; set; } = new();
}