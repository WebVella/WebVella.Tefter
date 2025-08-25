//using ClosedXML.Excel;

//namespace WebVella.Tefter.Talk.Components;

///// <summary>
///// Description attribute is needed when presenting the component to the user as a select option
///// Localization attributes is needed to strongly type the location of the components translation resource
///// </summary>
//[LocalizationResource("WebVella.Tefter.Talk.Addons.SpaceViewColumnComponents.TalkCommentsCountComponent.TfTalkCommentsCountComponent", "WebVella.Tefter.Talk")]
//public partial class TfTalkCommentsCountComponent : TucBaseViewColumn<TfTalkCommentsCountComponentOptions>
//{
//	public const string ID = "5f3855f1-4819-488f-b24a-d4a81448e4f0";
//	public const string NAME = "Talk Comments Count Display";
//	public const string DESCRIPTION = "";
//	public const string FLUENT_ICON_NAME = "";

//	#region << Injects >>
//	[Inject] protected ITalkService TalkService { get; set; }
//	[Inject] protected IState<TfAuxDataState> TfAuxDataState { get; set; }
//	#endregion

//	#region << Constructor >>
//	/// <summary>
//	/// Needed because of the custom constructor
//	/// </summary>
//	[ActivatorUtilitiesConstructor]
//	public TfTalkCommentsCountComponent()
//	{
//	}

//	/// <summary>
//	/// The custom constructor is needed because in varoius cases we need to instance the component without
//	/// rendering. The export to excel is one of those cases.
//	/// </summary>
//	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
//	public TfTalkCommentsCountComponent(TfSpaceViewColumnScreenRegionContext context)
//	{
//		RegionContext = context;
//	}
//	#endregion

//	#region << Properties >>
//	public override Guid AddonId { get; init; } = new Guid(ID);
//	public override string AddonName { get; init; } = NAME;
//	public override string AddonDescription { get; init; } = DESCRIPTION;
//	public override string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
//	public override List<Guid> SupportedColumnTypes { get; init; } = new List<Guid>{
//		new Guid(TfTalkCommentsCountViewColumnType.ID)
//	};
//	private List<TucSelectOption> _joinKeyOptions = new();
//	/// <summary>
//	/// Each state has an unique hash and this is set in the component context under the Hash property value
//	/// </summary>
//	private string _renderedHash = null;
//	private string _storageKey = "";
//	private IDialogReference _dialog;
//	private List<TalkChannel> _channels = new();
//	private TalkChannel _selectedChannel = null;
//	#endregion

//	#region << Lifecycle >>
//	protected override async Task OnInitializedAsync()
//	{
//		await base.OnInitializedAsync();
//		_initStorageKeys();
//		if (RegionContext.Mode == TfComponentPresentationMode.Options)
//		{
//			_channels = TalkService.GetChannels();

//			if (componentOptions.ChannelId is not null)
//			{
//				if (_channels.Count > 0)
//				{
//					var selectedIndex = _channels.FindIndex(x => x.Id == componentOptions.ChannelId);
//					if (selectedIndex == -1)
//					{
//						//This channel was probably deleted
//						componentOptions.ChannelId = null;
//						await OnOptionsChanged(nameof(TfTalkCommentsCountComponentOptions.ChannelId), (Guid?)null);
//					}
//					else
//					{
//						_selectedChannel = _channels[selectedIndex];
//					}
//				}
//			}
//			else
//			{
//				if (_channels.Count > 0)
//				{
//					await OnOptionsChanged(nameof(TfTalkCommentsCountComponentOptions.ChannelId), _channels[0].Id);
//					_selectedChannel = _channels[0];
//				}
//			}
//		}
//	}

//	/// <summary>
//	/// When data needs to be inited, parameter set is the best place as Initialization is 
//	/// done only once
//	/// </summary>
//	protected override async Task OnParametersSetAsync()
//	{
//		await base.OnParametersSetAsync();
//		var contextHash = RegionContext.GetHash();
//		if (contextHash != _renderedHash)
//		{
//			_initValues();
//			_renderedHash = contextHash;
//		}
//	}
//	#endregion

//	#region << Non rendered methods >>
//	/// <summary>
//	/// Overrides the default export method in order to apply its own options
//	/// </summary>
//	/// <returns></returns>
//	public override void ProcessExcelCell(IServiceProvider serviceProvider,IXLCell excelCell)
//	{
//		return;
//	}

//	public override Task OnAppStateInit(
//		IServiceProvider serviceProvider,
//		TucUser currentUser,
//		TfAppState newAppState, TfAppState oldAppState,
//		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
//	{
//		_initStorageKeys();
//		var options = new List<TucSelectOption>();
//		if (newAppState.SpaceView.SpaceDataId is not null)
//		{
//			var tfService = serviceProvider.GetRequiredService<ITfService>();
//			var spaceData = tfService.GetSpaceData(newAppState.SpaceView.SpaceDataId.Value);
//			if (spaceData is not null)
//			{
//				var dataProvider = tfService.GetDataProvider(spaceData.DataProviderId);
//				if (dataProvider is not null)
//				{
//					foreach (var key in dataProvider.JoinKeys)
//					{
//						options.Add(new TucSelectOption
//						{
//							Value = key.DbName,
//							Label = key.DbName
//						});
//					}
//				}
//			}
//		}
//		newAuxDataState.Data[_storageKey] = options;
//		return Task.CompletedTask;
//	}
//	#endregion
//	#region << Private logic >>
//	private async Task _onClick()
//	{
//		var panelContext = new TalkThreadPanelContext
//		{
//			ChannelId = componentOptions.ChannelId,
//			DataTable = RegionContext.DataTable,
//			RowIndex = RegionContext.RowIndex
//		};

//		_dialog = await DialogService.ShowPanelAsync<TalkThreadPanel>(
//		panelContext,
//		new DialogParameters()
//		{
//			DialogType = DialogType.Panel,
//			Alignment = HorizontalAlignment.Right,
//			ShowTitle = false,
//			ShowDismiss = false,
//			PrimaryAction = null,
//			SecondaryAction = null,
//			Width = "35vw",
//			TrapFocus = false
//		});
//	}

//	private void _initValues()
//	{
//		if (!TfAuxDataState.Value.Data.ContainsKey(_storageKey)) return;
//		_joinKeyOptions = ((List<TucSelectOption>)TfAuxDataState.Value.Data[_storageKey]).ToList();
//	}

//	private void _initStorageKeys()
//	{
//		_storageKey = this.GetType().Name + "_" + RegionContext.SpaceViewColumnId;
//	}

//	private async Task _channelSelectHandler(TalkChannel channel)
//	{
//		_selectedChannel = channel;
//		await OnOptionsChanged(nameof(TfTalkCommentsCountComponentOptions.ChannelId), channel?.Id);
//	}
//	#endregion

//}

//public class TfTalkCommentsCountComponentOptions
//{
//	public Guid? ChannelId { get; set; } = null;
//}