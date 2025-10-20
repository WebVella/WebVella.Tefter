namespace WebVella.Tefter.UI.Addons;

public partial class TucSelectViewColumnTypeEdit : TfLocalizedViewColumnComponent
{
	[Inject] protected IJSRuntime JsRuntime { get; set; } = null!;	
	[Parameter] public TfSpaceViewColumnEditModeContext Context { get; set; } = null!;
	[Parameter] public TfSelectOption? Value { get; set; }
	[Parameter] public List<TfSelectOption> Options { get; set; } = new();
	[Parameter] public EventCallback<TfSelectOption?> ValueChanged { get; set; }


	private readonly string _valueInputId = "input-" + Guid.NewGuid();
	private bool _open = false;

	private async Task _valueChanged(TfSelectOption? value)
	{
		var settings = Context.GetSettings<TfSelectViewColumnTypeSettings>();
		if (!String.IsNullOrWhiteSpace(settings.ChangeConfirmationMessage)
		    && !await JsRuntime.InvokeAsync<bool>("confirm", settings.ChangeConfirmationMessage))
			return;
		
		Value = value;
		await ValueChanged.InvokeAsync(Value);
		await JsRuntime.InvokeAsync<string>("Tefter.blurElementById", _valueInputId);
	}
}