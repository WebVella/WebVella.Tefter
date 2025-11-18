namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewPinColumnsDialog : TfBaseComponent, IDialogContentComponent<TfSpaceView?>
{
	[Parameter] public TfSpaceView? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private List<TfSpaceViewColumn> _viewColumns = new();
	private List<TfSpaceViewColumn> _reverseColumns = new();
	private Dictionary<Guid, Tuple<int, bool>> _columnFromLeftMeta = new();
	private Dictionary<Guid, Tuple<int, bool>> _columnFromRightMeta = new();
	private int? _leftPosition = null;
	private int? _rightPosition = null;


	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) throw new Exception("Content Id is required");
		_viewColumns = TfService.GetSpaceViewColumnsList(Content.Id);
		_leftPosition = Content.Settings.FreezeStartingNColumns;
		_rightPosition = Content.Settings.FreezeFinalNColumns;
		_initDictionaries();
	}

	private void _initDictionaries()
	{
		_reverseColumns = _viewColumns.AsEnumerable().Reverse().ToList();
		int position = 1;
		foreach (var column in _viewColumns)
		{
			_columnFromLeftMeta[column.Id] = new Tuple<int, bool>(
				item1: position,
				item2: _leftPosition is not null && position <= _leftPosition.Value);
			position++;
		}

		position = 1;
		foreach (var column in _reverseColumns)
		{
			_columnFromRightMeta[column.Id] = new Tuple<int, bool>(
				item1: position,
				item2: _rightPosition is not null && position <= _rightPosition);
			position++;
		}
	}

	private void _columnSelected(TfSpaceViewColumn column, bool isStart)
	{
		//Start
		if (isStart)
		{
			var meta = _columnFromLeftMeta[column.Id];
			//Case 1: checked
			if (meta.Item2)
			{
				//Case 1: start position > item position - make item current
				if (_leftPosition > meta.Item1)
				{
					_leftPosition = meta.Item1;
				}
				//Case 1: start position == item position - make previous one current
				else
				{
					_leftPosition = meta.Item1 - 1;
					if (_leftPosition <= 0) _leftPosition = null;
				}
			}
			//Case 2: NOT checked
			else
			{
				_leftPosition = meta.Item1;
			}
		}
		//End
		else
		{
			var meta = _columnFromRightMeta[column.Id];
			//Case 1: checked
			if (meta.Item2)
			{
				//Case 1: end position > item position - make item current
				if (_rightPosition > meta.Item1)
				{
					_rightPosition = meta.Item1;
				}
				//Case 1: start position == item position - make previous one current
				else
				{
					_rightPosition = meta.Item1 - 1;
					if (_rightPosition <= 0) _rightPosition = null;
				}
			}
			//Case 2: NOT checked
			else
			{
				_rightPosition = meta.Item1;
			}		
		}

		_initDictionaries();
	}

	private async Task _save()
	{
		if (_isSubmitting) return;
		try
		{
			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);

			await TfService.UpdateSpaceViewSettings(Content!.Id, Content.Settings with
			{
				FreezeStartingNColumns = _leftPosition,
				FreezeFinalNColumns = _rightPosition
			});

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