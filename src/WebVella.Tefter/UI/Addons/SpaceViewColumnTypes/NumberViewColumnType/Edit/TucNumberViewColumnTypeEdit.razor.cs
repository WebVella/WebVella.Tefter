namespace WebVella.Tefter.UI.Addons;

public partial class TucNumberViewColumnTypeEdit : TfLocalizedViewColumnComponent
{
	[Inject] protected IJSRuntime JsRuntime { get; set; } = null!;
	[Inject] protected IToastService ToastService { get; set; } = null!;

	[Parameter]
	public decimal? Value
	{
		get => _original;
		set
		{
			_original = value;
			_value = value;
		}
	}

	[Parameter] public EventCallback<decimal?> ValueChanged { get; set; }
	[Parameter] public TfNumberViewColumnTypeSettings Settings { get; set; } = null!;

	private readonly string _valueInputId = "input-" + Guid.NewGuid();

	private decimal? _original = null;
	private decimal? _value = null;

	private async Task _valueChanged()
	{
		if (!String.IsNullOrWhiteSpace(Settings.ChangeConfirmationMessage)
		    && !await JsRuntime.InvokeAsync<bool>("confirm", Settings.ChangeConfirmationMessage))
			return;

		Value = _value;
		await ValueChanged.InvokeAsync(Value);
		await JsRuntime.InvokeAsync<string>("Tefter.blurElementById", _valueInputId);
	}
}