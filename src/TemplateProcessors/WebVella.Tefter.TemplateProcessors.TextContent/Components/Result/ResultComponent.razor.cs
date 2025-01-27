namespace WebVella.Tefter.TemplateProcessors.TextContent.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.TextContent.Components.Result.ResultComponent", "WebVella.Tefter.TemplateProcessors.TextContent")]
public partial class ResultComponent : TfBaseComponent, 
	ITfRegionComponent<TfTemplateProcessorResultComponentContext>,
	ITfComponentScope<TextContentTemplateProcessor>
{
	[Inject] private ITfTemplateService TemplateService { get; set; }

	public Guid Id { get; init; } = new Guid("ac4317e2-ef23-46b8-875d-c763e50a5d8e");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "Text Content Result";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	[Parameter] public TfTemplateProcessorResultComponentContext Context { get; init; }


	private TextContentTemplateResult _result = null;
	private bool _isLoading = true;
	private bool _showDetails = false;
	private TextContentTemplateResultItem _form = new();
	private Dictionary<Guid, int> _itemPositionDict = new();
	private int _itemPosition = 1;
	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (Context is null) throw new Exception("Context is not defined");
	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			if (Context.Template is not null && Context.SpaceData is not null)
			{
				ITfTemplateResult result = TemplateService.ProcessTemplate(
					templateId: Context.Template.Id,
					spaceDataId: Context.SpaceData.Id,
					tfRecordIds: Context.SelectedRowIds,
					preview: Context.Preview
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


