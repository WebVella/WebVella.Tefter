namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewColorRulesDialog : TfBaseComponent, IDialogContentComponent<TfSpaceView?>
{
	[Parameter] public TfSpaceView? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private List<TfColoringRule> _rules = new();
	private List<TfSpaceViewColumn> _viewColumns = new();
	private TfDataProvider _provider = new();
	private List<TfDataProvider> _allProviders = new();
	private List<TfSharedColumn> _allColumns = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) throw new Exception("Content Id is required");
		_viewColumns = TfService.GetSpaceViewColumnsList(Content.Id);
		var dataSet = TfService.GetDataset(Content.DatasetId);
		if (dataSet is null) throw new Exception("Dataset not found");
		_allProviders = TfService.GetDataProviders().ToList();
		_provider = _allProviders.Single(x => x.Id == dataSet.DataProviderId);
		_allColumns = TfService.GetSharedColumns();
		_rules = Content.Settings.ColoringRules;
	}

	private async Task _addRule()
	{
		//TucSpaceViewManageColorRuleDialog
		var dialog = await DialogService.ShowDialogAsync<TucSpaceViewManageColorRuleDialog>(
			new TfSpaceViewManageColorRuleDialogContext
			{
				Rule = new TfColoringRule() { Position = _rules.Count + 1 },
				View = Content!,
				Columns = _viewColumns,
				Provider = _provider,
				AllProvider = _allProviders,
				AllSharedColumns = _allColumns,
			},
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthExtraLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			_rules.Add((TfColoringRule)result.Data);
			for (int i = 0; i < _rules.Count; i++)
			{
				_rules[i].Position = i + 1;
			}
		}
	}

	private async Task _editRule(TfColoringRule rule)
	{
		var dialog = await DialogService.ShowDialogAsync<TucSpaceViewManageColorRuleDialog>(
			new TfSpaceViewManageColorRuleDialogContext
			{
				Rule = rule,
				View = Content!,
				Columns = _viewColumns,
				Provider = _provider,
				AllProvider = _allProviders,
				AllSharedColumns = _allColumns,
			},
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthExtraLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var editedRule = (TfColoringRule)result.Data;
			var ruleIndex = _rules.FindIndex(x => x.Id == editedRule.Id);
			if (ruleIndex < 0) throw new Exception("Rule not found");

			_rules[ruleIndex] = editedRule;
			for (int i = 0; i < _rules.Count; i++)
			{
				_rules[i].Position = i + 1;
			}
		}
	}

	private void _deleteRule(TfColoringRule rule)
	{
		_rules = _rules.Where(x => x.Id != rule.Id).ToList();
		for (int i = 0; i < _rules.Count; i++)
		{
			_rules[i].Position = i + 1;
		}
	}

	private void _moveRule(TfColoringRule rule, bool isUp)
	{
		if (rule.Position == 1 && isUp) return;
		if (rule.Position == _rules.Count && !isUp) return;

		_rules = _rules.Where(x => x.Id != rule.Id).ToList();
		if (isUp)
			_rules.Insert(rule.Position - 2, rule);
		else
			_rules.Insert(rule.Position, rule);
		
		for (int i = 0; i < _rules.Count; i++)
		{
			_rules[i].Position = i + 1;
		}
	}


	private async Task _save()
	{
		if (_isSubmitting) return;
		try
		{
			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);

			await TfService.UpdateSpaceViewSettings(Content!.Id, Content.Settings with {ColoringRules = _rules});

			await Dialog.CloseAsync();
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isSubmitting = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}
}