namespace WebVella.Tefter.UI.Addons;

public partial class TucLongIntegerViewColumnTypeEdit : TfLocalizedViewColumnComponent
{
	[Inject] protected IJSRuntime JsRuntime { get; set; } = null!;
	[Inject] protected IToastService ToastService { get; set; } = null!;
	[Parameter] public TfSpaceViewColumnEditMode Context { get; set; } = null!;
	[Parameter]
	public long? Value
	{
		get => _original;
		set
		{
			_original = value;
			_value = value;
		}
	}

	[Parameter] public EventCallback<long?> ValueChanged { get; set; }

	private readonly string _valueInputId = "input-" + Guid.NewGuid();

	private long? _original = null;
	private long? _value = null;

	private async Task _valueChanged()
	{
		var settings = Context.GetSettings<TfLongIntegerViewColumnTypeSettings>();
		if (!String.IsNullOrWhiteSpace(settings.ChangeConfirmationMessage)
		    && !await JsRuntime.InvokeAsync<bool>("confirm", settings.ChangeConfirmationMessage))
			return;

		Value = _value;
		await ValueChanged.InvokeAsync(Value);
		await JsRuntime.InvokeAsync<string>("Tefter.blurElementById", _valueInputId);
	}
}