namespace WebVella.Tefter.UI.Addons;

public partial class TucSelectViewColumnTypeEdit : TfLocalizedViewColumnComponent
{
	[Inject] protected IJSRuntime JsRuntime { get; set; } = null!;	
	[Parameter] public TfSelectOption? Value { get; set; }
	[Parameter] public List<TfSelectOption> Options { get; set; } = new();
	[Parameter] public EventCallback<TfSelectOption?> ValueChanged { get; set; }
	[Parameter] public TfSelectViewColumnTypeSettings Settings { get; set; } = null!;

	private readonly string _valueInputId = "input-" + Guid.NewGuid();
	private bool _open = false;

	private async Task _valueChanged(TfSelectOption? value)
	{
		if (!String.IsNullOrWhiteSpace(Settings.ChangeConfirmationMessage)
		    && !await JsRuntime.InvokeAsync<bool>("confirm", Settings.ChangeConfirmationMessage))
			return;
		
		Value = value;
		await ValueChanged.InvokeAsync(Value);
		await JsRuntime.InvokeAsync<string>("Tefter.blurElementById", _valueInputId);
	}
}