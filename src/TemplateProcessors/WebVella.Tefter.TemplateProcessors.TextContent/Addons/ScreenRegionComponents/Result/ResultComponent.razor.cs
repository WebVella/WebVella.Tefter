namespace WebVella.Tefter.TemplateProcessors.TextContent.Addons;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.TextContent.Addons.ScreenRegionComponents.Result.ResultComponent", "WebVella.Tefter.TemplateProcessors.TextContent")]
public partial class ResultComponent : TfBaseComponent, 
	ITfScreenRegionComponent<TfTemplateProcessorResultScreenRegionContext>
{
	public const string ID = "ac4317e2-ef23-46b8-875d-c763e50a5d8e";
	public const string NAME = "Text Content Result";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "PuzzlePiece";
	public const int POSITION_RANK = 1000;
	public Guid Id { get; init; } = new Guid(ID);
	public string Name { get; init; } = NAME;
	public string Description { get; init; } = DESCRIPTION;
	public string FluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(TextContentTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorResultScreenRegionContext RegionContext { get; init; }


	private TextContentTemplateResult _result = null;
	private bool _isLoading = true;
	private TextContentTemplateResultItem _form = new();
	private Dictionary<Guid, int> _itemPositionDict = new();
	private int _itemPosition = 1;
	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (RegionContext is null) throw new Exception("Context is not defined");
	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			if (RegionContext.Template is not null && RegionContext.SpaceData is not null)
			{
				ITfTemplateResult result = TfService.ProcessTemplate(
					templateId: RegionContext.Template.Id,
					spaceDataId: RegionContext.SpaceData.Id,
					tfRecordIds: RegionContext.SelectedRowIds,
					preview: RegionContext.Preview
				);
				if (result is not TextContentTemplateResult)
				{
					throw new Exception("Preview result is not of type TextContentTemplateResult");
				}

				_result = (TextContentTemplateResult)result;
				_itemPositionDict = new();
				if (_result.Items.Count > 0)
				{
					_form = _result.Items[0];
					var position = 1;
					foreach (var item in _result.Items)
					{
						_itemPositionDict[item.Id] = position;
						position++;
					}
				}
			}

			_isLoading = false;
			StateHasChanged();
		}
	}

	private void _nextItem()
	{
		if (_form is null || _itemPositionDict is null) return;
		var itemPosition = _itemPositionDict[_form.Id];
		var newPosition = itemPosition + 1;
		if (newPosition <= 0 || newPosition > _result.Items.Count)
			newPosition = 1;
		_form = _result.Items[newPosition - 1];
	}
	private void _prevItem()
	{
		if (_form is null || _itemPositionDict is null) return;
		var itemPosition = _itemPositionDict[_form.Id];
		var newPosition = itemPosition - 1;
		if (newPosition <= 0)
			newPosition = _result.Items.Count;
		_form = _result.Items[newPosition - 1];
	}

	private void _selectedOptionChanged(TextContentTemplateResultItem item)
	{
		_form = item;
	}

	private async Task _copyToClipboard()
	{
		if (_form is null) return;
		await JSRuntime.InvokeVoidAsync("Tefter.copyToClipboard", _form.Content);
		ToastService.ShowSuccess(LOC("Content copied"));
	}
}


