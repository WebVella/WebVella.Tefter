namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.SelectList.TfSelectList", "WebVella.Tefter")]
public partial class TucSelectList : TfBaseComponent
{
	[Parameter]
	public List<string> Values { get; set; } = new();

	[Parameter]
	public EventCallback<List<string>> ValuesChanged { get; set; }

	[Parameter] public string Placeholder { get; set; }

	[Parameter] public string NoItemsMessage { get; set; }

	private bool _isReadonly { get => !ValuesChanged.HasDelegate; }
	private string _noItemsMessage { get => String.IsNullOrWhiteSpace(NoItemsMessage) ? LOC("no items") : NoItemsMessage; }

	public string _inputValue = null;

	private async Task _onInputValueChanged(string x)
	{
		if (String.IsNullOrWhiteSpace(x)) return;
		_inputValue = x;
		await InvokeAsync(StateHasChanged);
		await Task.Delay(5);
		_inputValue = null;
		await InvokeAsync(StateHasChanged);
		var values = Values.ToList();
		values.Add(x);
		await ValuesChanged.InvokeAsync(values);
	}

	private async Task _onRemoveColumn(string x){ 
		if (String.IsNullOrWhiteSpace(x)) return;
		if(!Values.Contains(x)) return;
		var values = Values.ToList();
		values.Remove(x);
		await ValuesChanged.InvokeAsync(values);

	}

	private async Task _addBtnClicked(){ 
		await Task.Delay(10);
	}
}