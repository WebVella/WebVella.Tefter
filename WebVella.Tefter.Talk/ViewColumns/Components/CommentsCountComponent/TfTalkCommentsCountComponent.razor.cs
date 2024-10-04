﻿namespace WebVella.Tefter.Talk.Components;

/// <summary>
/// Description attribute is needed when presenting the component to the user as a select option
/// Localization attributes is needed to strongly type the location of the components translation resource
/// </summary>
[Description("Talk Comments Count Display")]
[LocalizationResource("WebVella.Tefter.Talk.ViewColumns.Components.TalkCommentsCountComponent.TfTalkCommentsCountComponent", "WebVella.Tefter")]
public partial class TfTalkCommentsCountComponent : TucBaseViewColumn<TfTalkCommentsCountComponentOptions>, ITucAuxDataUseComponent
{
	#region << Injects >>
	[Inject] protected IState<TfAuxDataState> TfAuxDataState { get; set; }
	#endregion

	#region << Constructor >>
	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	public TfTalkCommentsCountComponent()
	{
	}

	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
	public TfTalkCommentsCountComponent(TucViewColumnComponentContext context)
	{
		Context = context;
	}
	#endregion

	#region << Properties >>
	private int? _value = null;
	private List<TucSelectOption> _sharedKeyOptions = new();
	/// <summary>
	/// Each state has an unique hash and this is set in the component context under the Hash property value
	/// </summary>
	private Guid? _renderedHash = null;
	private string _storageKey = "";
	private IDialogReference? _dialog;
	#endregion

	#region << Lifecycle >>
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_initStorageKeys();
	}

	/// <summary>
	/// When data needs to be inited, parameter set is the best place as Initialization is 
	/// done only once
	/// </summary>
	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		if (Context.Hash != _renderedHash)
		{
			_initValues();
			_renderedHash = Context.Hash;
		}
	}
	#endregion

	#region << Non rendered methods >>
	/// <summary>
	/// Overrides the default export method in order to apply its own options
	/// </summary>
	/// <returns></returns>
	public override object GetData(IServiceProvider serviceProvider)
	{
		return null;
	}

	public override async Task OnSpaceViewStateInited(
		IServiceProvider serviceProvider,
		TucUser currentUser,
		TfRouteState routeState,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	{
		_initStorageKeys();
		var options = new List<TucSelectOption>();
		if (newAppState.SpaceView.SpaceDataId is not null)
		{
			var spaceManager = serviceProvider.GetRequiredService<ITfSpaceManager>();
			var dataProviderManager = serviceProvider.GetRequiredService<ITfDataProviderManager>();
			var spaceDataResult = spaceManager.GetSpaceData(newAppState.SpaceView.SpaceDataId.Value);
			if (spaceDataResult.IsSuccess && spaceDataResult.Value is not null)
			{
				var dataProviderResult = dataProviderManager.GetProvider(spaceDataResult.Value.DataProviderId);
				if (dataProviderResult.IsSuccess && dataProviderResult.Value is not null)
				{
					foreach (var key in dataProviderResult.Value.SharedKeys)
					{
						options.Add(new TucSelectOption
						{
							Value = key.DbName,
							Label = key.DbName
						});
					}
				}
			}
		}
		newAuxDataState.Data[_storageKey] = options;
	}
	#endregion
	#region << Private logic >>
	private async Task _onClick()
	{
		_dialog = await DialogService.ShowPanelAsync<TalkThreadPanel>(
		null,
		new DialogParameters()
		{
			DialogType = DialogType.Panel,
			Alignment = HorizontalAlignment.Right,
			ShowTitle = false,
			ShowDismiss = false,
			PrimaryAction = null,
			SecondaryAction = null,
			Width = "75vw"
		});
	}

	private void _initValues()
	{
		if (!TfAuxDataState.Value.Data.ContainsKey(_storageKey)) return;
		_sharedKeyOptions = ((List<TucSelectOption>)TfAuxDataState.Value.Data[_storageKey]).ToList();
	}

	private void _initStorageKeys()
	{
		_storageKey = this.GetType().Name + "_" + Context.SpaceViewColumnId;
	}
	#endregion

}

public class TfTalkCommentsCountComponentOptions
{
	public string SharedKeyName { get; set; }
}