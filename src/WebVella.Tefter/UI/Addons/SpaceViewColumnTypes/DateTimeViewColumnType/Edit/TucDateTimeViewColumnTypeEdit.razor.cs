namespace WebVella.Tefter.UI.Addons;

public partial class TucDateTimeViewColumnTypeEdit : TfLocalizedViewColumnComponent
{
	[Inject] protected IJSRuntime JsRuntime { get; set; } = null!;
	[Inject] protected IToastService ToastService { get; set; } = null!;

	[Parameter]
	public DateTime? Value
	{
		get => _original;
		set
		{
			_original = value;
			_value = value.ToDateTime();
		}
	}

	[Parameter] public EventCallback<DateTime?> ValueChanged { get; set; }
	[Parameter] public TfDateTimeViewColumnTypeSettings Settings { get; set; } = null!;

	private readonly string _valueInputId = "input-" + Guid.NewGuid();
	private readonly string _valueTimeInputId = "input-" + Guid.NewGuid();

	private DateTime? _original = null;
	private DateTime? _value = null;

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