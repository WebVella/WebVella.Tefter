namespace WebVella.Tefter.UI.Addons;

public partial class TucNumberViewColumnTypeEdit : TfLocalizedViewColumnComponent
{
	[Inject] protected IJSRuntime JsRuntime { get; set; } = null!;
	[Inject] protected IToastService ToastService { get; set; } = null!;
	[Parameter] public TfSpaceViewColumnEditMode Context { get; set; } = null!;
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

	private readonly string _valueInputId = "input-" + Guid.NewGuid();

	private decimal? _original = null;
	private decimal? _value = null;

	private async Task _valueChanged()
	{
		var settings = Context.GetSettings<TfNumberViewColumnTypeSettings>();
		if (!String.IsNullOrWhiteSpace(settings.ChangeConfirmationMessage)
		    && !await JsRuntime.InvokeAsync<bool>("confirm", settings.ChangeConfirmationMessage))
			return;

		Value = _value;
		await ValueChanged.InvokeAsync(Value);
		await JsRuntime.InvokeAsync<string>("Tefter.blurElementById", _valueInputId);
	}
}