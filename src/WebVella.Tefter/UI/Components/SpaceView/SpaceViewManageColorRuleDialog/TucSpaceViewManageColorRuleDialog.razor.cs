namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewManageColorRuleDialog : TfBaseComponent, IDialogContentComponent<TfSpaceViewManageColorRuleDialogContext?>
{
	[Parameter] public TfSpaceViewManageColorRuleDialogContext? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private List<TfColoringRule> _rules = new();
	private TfColoringRule _form;
	private List<ValidationError> _validationErrors = new();
	private TfSpaceViewColumn? _selectedOption = null;
	private List<TfSpaceViewColumn> _options = new();
	private List<TfSpaceViewColumn> _selectedOptions = new();
	
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.View.Id == Guid.Empty) throw new Exception("Content View Id is required");
		_rules = new List<TfColoringRule>() { };
		_form = Content.Rule with {Id = Content.Rule.Id};
		_initOptions();
	}

	private void _initOptions()
	{
		_options = Content!.Columns.Where(x=> !_form.Columns.Contains(x.QueryName)).ToList();
		_selectedOptions = Content.Columns.Where(x=> _form.Columns.Contains(x.QueryName)).OrderBy(x=> x.Title).ToList();
	}

	private void _addColumnHandler(TfSpaceViewColumn col)
	{
		if(_form.Columns.Contains(col.QueryName)) return;
		_form.Columns.Add(col.QueryName);
		_initOptions();
		_selectedOption = null;
	}

	private void _removeColumnHandler(TfSpaceViewColumn col)
	{
		if(!_form.Columns.Contains(col.QueryName)) return;
		_form.Columns.Remove(col.QueryName);
		_initOptions();
	}	
	
	private async Task _save()
	{
		await Dialog.CloseAsync(_form);
	}

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}
}

public record TfSpaceViewManageColorRuleDialogContext
{
	public TfColoringRule Rule { get; set; } = new();
	public TfSpaceView View { get; set; } = new();
	public List<TfSpaceViewColumn> Columns { get; set; } = new();
	public TfDataProvider Provider { get; set; } = new();
	public List<TfDataProvider> AllProvider { get; set; } = new();
	public List<TfSharedColumn> AllSharedColumns { get; set; } = new();
}