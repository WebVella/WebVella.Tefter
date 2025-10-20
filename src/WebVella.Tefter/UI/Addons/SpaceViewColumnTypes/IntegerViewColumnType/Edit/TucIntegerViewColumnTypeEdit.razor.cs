namespace WebVella.Tefter.UI.Addons;

public partial class TucIntegerViewColumnTypeEdit : TfLocalizedViewColumnComponent
{
	[Inject] protected IJSRuntime JsRuntime { get; set; } = null!;
	[Inject] protected IToastService ToastService { get; set; } = null!;
	[Parameter] public TfSpaceViewColumnEditModeContext Context { get; set; } = null!;
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

	private readonly string _valueInputId = "input-" + Guid.NewGuid();

	private int? _original = null;
	private int? _value = null;

	private async Task _valueChanged()
	{
		var settings = Context.GetSettings<TfIntegerViewColumnTypeSettings>();
		if (!String.IsNullOrWhiteSpace(settings.ChangeConfirmationMessage)
		    && !await JsRuntime.InvokeAsync<bool>("confirm", settings.ChangeConfirmationMessage))
			return;

		Value = _value;
		await ValueChanged.InvokeAsync(Value);
		await JsRuntime.InvokeAsync<string>("Tefter.blurElementById", _valueInputId);
	}
}