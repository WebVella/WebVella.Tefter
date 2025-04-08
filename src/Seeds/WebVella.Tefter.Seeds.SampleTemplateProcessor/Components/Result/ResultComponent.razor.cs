namespace WebVella.Tefter.Seeds.SampleTemplateProcessor.Components;

public partial class ResultComponent : TfBaseComponent, 
	ITfRegionComponent<TfTemplateProcessorResultComponentContext>
{
	public Guid Id { get; init; } = new Guid("ac4317e2-ef23-46b8-875d-c763e50a5d8e");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "Text Content Result";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	public List<TfRegionComponentScope> Scopes { get; init; } = new List<TfRegionComponentScope>(){ 
		new TfRegionComponentScope(typeof(SampleTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorResultComponentContext Context { get; init; }


	private SampleTemplateResult _result = null;
	private bool _isLoading = true;
	private SampleTemplateResultItem _form = new();
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
				ITfTemplateResult result = TfService.ProcessTemplate(
					templateId: Context.Template.Id,
					spaceDataId: Context.SpaceData.Id,
					tfRecordIds: Context.SelectedRowIds,
					preview: Context.Preview
				);
				if (result is not SampleTemplateResult)
				{
					throw new Exception("Preview result is not of type SampleTemplateResult");
				}

				_result = (SampleTemplateResult)result;
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

	private void _selectedOptionChanged(SampleTemplateResultItem item)
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


