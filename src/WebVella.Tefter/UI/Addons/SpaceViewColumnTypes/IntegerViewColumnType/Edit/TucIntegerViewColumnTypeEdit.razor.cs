namespace WebVella.Tefter.UI.Addons;

public partial class TucIntegerViewColumnTypeEdit : TfLocalizedViewColumnComponent
{
	[Inject] protected IJSRuntime JsRuntime { get; set; } = null!;
	[Inject] protected IToastService ToastService { get; set; } = null!;

	[Parameter]
	public int? Value
	{
		get => _original;
		set
		{
			_original = value;
			_value = value;
		}
	}

	[Parameter] public EventCallback<int?> ValueChanged { get; set; }
	[Parameter] public TfIntegerViewColumnTypeSettings Settings { get; set; } = null!;

	private readonly string _valueInputId = "input-" + Guid.NewGuid();

	private int? _original = null;
	private int? _value = null;

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