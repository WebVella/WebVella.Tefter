using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.Talk.Addons;
[LocalizationResource("WebVella.Tefter.Talk.Addons.SpacePages.TalkSpacePage.TalkSpacePageComponent", "WebVella.Tefter.Talk")]
public partial class TalkSpacePageComponent : TucBaseSpacePageComponent
{
	public const string ID = "6589259a-4f90-445d-8a62-d4b51bab3afd";
	public const string NAME = "Talk Page";
	public const string DESCRIPTION = "general discussion per page";
	public const string FLUENT_ICON_NAME = "ChatMultiple";

	#region << Injects >>
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] protected IState<TfAuxDataState> TfAuxDataState { get; set; }
	[Inject] protected ITalkService TalkService { get; set; }
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

	public override Task<(TfAppState, TfAuxDataState)> InitState(
		IServiceProvider serviceProvider,
		TucUser currentUser,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState,
		TfSpacePageAddonContext context)
	{
		//var talkService = serviceProvider.GetService<ITalkService>();
		//var allChannelsResult = talkService.GetChannels();
		//if(allChannelsResult.IsFailed) throw new Exception("GetChannels failed");

		//newAuxDataState.Data[contextKeyInAuxDataState] = new TalkPageComponentPageComponentContext{ 
		//	TalkChannels = allChannelsResult.Value
		//};
		return Task.FromResult((newAppState, newAuxDataState));
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
	private string optionsJson = "{}";
	private TalkSpacePageComponentOptions _options { get; set; } = new();
	private TalkChannel _optionsChannel { get; set; } = null;
	private List<TalkChannel> _channels { get; set; } = new();

	#endregion


	#region << Render Lifecycle >>
	protected override void OnInitialized()
	{
		base.OnInitialized();
		_channels = TalkService.GetChannels();
		if(!String.IsNullOrWhiteSpace(Context.ComponentOptionsJson)){ 
			optionsJson = Context.ComponentOptionsJson;
			_options = JsonSerializer.Deserialize<TalkSpacePageComponentOptions>(optionsJson);
			if(_options.ChannelId is not null){ 
				_optionsChannel = _channels.FirstOrDefault(x=> x.Id == _options.ChannelId);
			}
		}

		if (_optionsChannel is null && _channels.Count > 0)
		{
			_optionsChannel = _channels[0];
			_options.ChannelId = _optionsChannel.Id;
		}
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Context.ComponentOptionsJson != optionsJson)
		{
			optionsJson = Context.ComponentOptionsJson;
			_options = JsonSerializer.Deserialize<TalkSpacePageComponentOptions>(optionsJson);
			//When cannot node has json from another page type
			if(_options is null) _options = new();
			if(_options.ChannelId is null && _optionsChannel is not null) _options.ChannelId = _optionsChannel.Id;
		}
	}
	#endregion

	#region << Private methods >>


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
}

public class TalkSpacePageComponentContext
{
	[JsonPropertyName("TalkChannels")]
	public List<TalkChannel> TalkChannels { get; set; } = new();
}