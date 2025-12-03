namespace WebVella.Tefter.UI.Addons;

public partial class TucDateViewColumnTypeEdit : TfLocalizedViewColumnComponent
{
	[Inject] protected IJSRuntime JsRuntime { get; set; } = null!;
	[Inject] protected IToastService ToastService { get; set; } = null!;
	[Parameter] public TfSpaceViewColumnEditMode Context { get; set; } = null!;
	[Parameter]
	public DateOnly? Value
	{
		get => _original;
		set
		{
			_original = value;
			_value = value.ToDateTime();
		}
	}

	[Parameter] public EventCallback<DateOnly?> ValueChanged { get; set; }
	
	private readonly string _valueInputId = "input-" + Guid.NewGuid();

	private DateOnly? _original = null;
	private DateTime? _value = null;

	private TfDateViewColumnTypeSettings _settings =  new ();

	protected override void OnParametersSet()
	{
		_settings = Context.GetSettings<TfDateViewColumnTypeSettings>();
	}	
	
	private async Task _valueChanged()
	{
		if (!String.IsNullOrWhiteSpace(_settings.ChangeConfirmationMessage)
		    && !await JsRuntime.InvokeAsync<bool>("confirm", _settings.ChangeConfirmationMessage))
			return;
		DateOnly? value = null;
		if (_value is not null)
		{
			value = new DateOnly(_value.Value.Year, _value.Value.Month, _value.Value.Day);
		}
		Value = value;
		await ValueChanged.InvokeAsync(Value);
		try
		{
			await JsRuntime.InvokeAsync<string>("Tefter.blurElementById", _valueInputId);
		}
		catch (Exception)
		{
			//ignored
		}		
	}
}