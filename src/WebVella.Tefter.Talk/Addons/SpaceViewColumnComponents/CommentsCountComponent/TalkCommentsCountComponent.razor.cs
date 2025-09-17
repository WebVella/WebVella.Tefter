using ClosedXML.Excel;

namespace WebVella.Tefter.Talk.Components;

public partial class TalkCommentsCountComponent : TucBaseViewColumn<TfTalkCommentsCountComponentOptions>
{
	public const string ID = "5f3855f1-4819-488f-b24a-d4a81448e4f0";
	public const string NAME = "Talk Comments Count Display";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "";

	#region << Injects >>
	[Inject] protected ITalkService TalkService { get; set; }
	#endregion

	#region << Constructor >>
	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	[ActivatorUtilitiesConstructor]
	public TalkCommentsCountComponent()
	{
	}

	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
	public TalkCommentsCountComponent(TfSpaceViewColumnScreenRegionContext context)
	{
		RegionContext = context;
	}
	#endregion

	#region << Properties >>
	public override Guid AddonId { get; init; } = new Guid(ID);
	public override string AddonName { get; init; } = NAME;
	public override string AddonDescription { get; init; } = DESCRIPTION;
	public override string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public override List<Guid> SupportedColumnTypes { get; init; } = new List<Guid>{
		new Guid(TfTalkCommentsCountViewColumnType.ID)
	};

	/// <summary>
	/// Each state has an unique hash and this is set in the component context under the Hash property value
	/// </summary>
	private string _renderedHash = null;
	private string _storageKey = "";
	private IDialogReference _dialog;
	private List<TalkChannel> _channels = new();
	private TalkChannel _selectedChannel = null;
	private long? _value = null;
	#endregion

	#region << Lifecycle >>
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_initStorageKeys();
		await _initValues();
	}

	/// <summary>
	/// When data needs to be inited, parameter set is the best place as Initialization is 
	/// done only once
	/// </summary>
	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		var contextHash = RegionContext.GetHash();
		if (contextHash != _renderedHash)
		{
			await _initValues();
			_renderedHash = contextHash;
		}
	}
	#endregion

	#region << Non rendered methods >>
	/// <summary>
	/// Overrides the default export method in order to apply its own options
	/// </summary>
	/// <returns></returns>
	public override void ProcessExcelCell(IServiceProvider serviceProvider, IXLCell excelCell)
	{
		return;
	}

    /// <summary>
    /// Overrides the default export method in order to apply its own options
    /// </summary>
    /// <returns></returns>
    public override string? GetValueAsString(IServiceProvider serviceProvider)
    {
		return null;
    }

    #endregion
    #region << Private logic >>
    private async Task _onClick()
	{
		var panelContext = new TalkThreadPanelContext
		{
			ChannelId = componentOptions.ChannelId,
			DataTable = RegionContext.DataTable,
			RowIndex = RegionContext.RowIndex
		};

		_dialog = await DialogService.ShowPanelAsync<TalkThreadPanel>(
		panelContext,
		new DialogParameters()
		{
			DialogType = DialogType.Panel,
			Alignment = HorizontalAlignment.Right,
			ShowTitle = false,
			ShowDismiss = false,
			PrimaryAction = null,
			SecondaryAction = null,
			Width = "35vw",
			TrapFocus = false,
			OnDialogClosing = EventCallback.Factory.Create<DialogInstance>(this, async (instance) =>
			{
				var change = ((TalkThreadPanelContext)instance.Content).CountChange;
				_value = (_value ?? 0) + change;
				if (_value <= 0) _value = null;
				await InvokeAsync(StateHasChanged);
			})
		});
	}

	private async Task _initValues()
	{
		if (RegionContext.Mode == TfComponentPresentationMode.Options)
		{
			_channels = TalkService.GetChannels();

			if (componentOptions.ChannelId is not null)
			{
				if (_channels.Count > 0)
				{
					var selectedIndex = _channels.FindIndex(x => x.Id == componentOptions.ChannelId);
					if (selectedIndex == -1)
					{
						//This channel was probably deleted
						componentOptions.ChannelId = null;
						await OnOptionsChanged(nameof(TfTalkCommentsCountComponentOptions.ChannelId), (Guid?)null);
					}
					else
					{
						_selectedChannel = _channels[selectedIndex];
					}
				}
			}
			else
			{
				if (_channels.Count > 0)
				{
					_selectedChannel = _channels[0];
				}
			}
			if (_selectedChannel is not null)
			{
				await OnOptionsChanged(nameof(TfTalkCommentsCountComponentOptions.ChannelId), _selectedChannel?.Id);
				await OnDataMappingChanged("Value", $"{_selectedChannel.DataIdentity}.{_selectedChannel.CountSharedColumnName}");
			}
			else{ 
				await OnOptionsChanged(nameof(TfTalkCommentsCountComponentOptions.ChannelId), (Guid?)null);
				await OnDataMappingChanged("Value", null);			
			}
		}
		else if (RegionContext.Mode == TfComponentPresentationMode.Display)
		{
			_selectedChannel = null;
			if (RegionContext.ViewData is not null
				&& RegionContext.ViewData.ContainsKey(_storageKey)
				&& RegionContext.ViewData[_storageKey] is TalkChannel)
			{
				_selectedChannel = (TalkChannel)RegionContext.ViewData[_storageKey];
			}
			else if (componentOptions.ChannelId is not null)
			{
				_selectedChannel = TalkService.GetChannel(componentOptions.ChannelId.Value);
				RegionContext.ViewData[_storageKey] = _selectedChannel;
			}
			_value = null;
			if (_selectedChannel is not null
				&& !String.IsNullOrWhiteSpace(_selectedChannel.DataIdentity)
				&& !String.IsNullOrWhiteSpace(_selectedChannel.CountSharedColumnName))
			{
				var columnName = $"{_selectedChannel.DataIdentity}.{_selectedChannel.CountSharedColumnName}";
				_value = (long?)GetDataStruct<long>(columnName, null);
			}

		}
	}

	private void _initStorageKeys()
	{
		_storageKey = this.GetType().Name + "_" + RegionContext.SpaceViewColumnId;
	}

	private async Task _channelSelectHandler(TalkChannel channel)
	{
		_selectedChannel = channel;
		await OnOptionsChanged(nameof(TfTalkCommentsCountComponentOptions.ChannelId), channel?.Id);
		await OnDataMappingChanged("Value", $"{channel.DataIdentity}.{channel.CountSharedColumnName}");
	}
	#endregion

}

public class TfTalkCommentsCountComponentOptions
{
	public Guid? ChannelId { get; set; } = null;
}