namespace WebVella.Tefter.UI.Addons;

public partial class TucTextViewColumnTypeEdit : TfLocalizedViewColumnComponent
{
	[Inject] protected IJSRuntime JsRuntime { get; set; } = null!;	
	[Parameter] public string? Value { get; set; }
	[Parameter] public EventCallback<string> ValueChanged { get; set; }
	[Parameter] public TfTextViewColumnTypeSettings Settings { get; set; } = null!;

	private readonly string _valueInputId = "input-" + Guid.NewGuid();


	private async Task _valueChanged(string? value)
	{
		if (!String.IsNullOrWhiteSpace(Settings.ChangeConfirmationMessage)
		    && !await JsRuntime.InvokeAsync<bool>("confirm", Settings.ChangeConfirmationMessage))
			return;
		
		Value = value;
		await ValueChanged.InvokeAsync(Value);
		await JsRuntime.InvokeAsync<string>("Tefter.blurElementById", _valueInputId);
	}
}