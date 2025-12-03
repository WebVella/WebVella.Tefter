namespace WebVella.Tefter.UI.Addons;

public partial class TucPhoneViewColumnTypeEdit : TfLocalizedViewColumnComponent
{
	[Inject] protected IJSRuntime JsRuntime { get; set; } = null!;	
	
	[Parameter] public TfSpaceViewColumnEditMode Context { get; set; } = null!;
	[Parameter] public string? Value { get; set; }
	[Parameter] public EventCallback<string> ValueChanged { get; set; }

	private readonly string _valueInputId = "input-" + Guid.NewGuid();


	private async Task _valueChanged(string? value)
	{
		var settings = Context.GetSettings<TfPhoneViewColumnTypeSettings>();
		if (!String.IsNullOrWhiteSpace(settings.ChangeConfirmationMessage)
		    && !await JsRuntime.InvokeAsync<bool>("confirm", settings.ChangeConfirmationMessage))
			return;
		
		Value = value;
		await ValueChanged.InvokeAsync(Value);
		await JsRuntime.InvokeAsync<string>("Tefter.blurElementById", _valueInputId);
	}
}