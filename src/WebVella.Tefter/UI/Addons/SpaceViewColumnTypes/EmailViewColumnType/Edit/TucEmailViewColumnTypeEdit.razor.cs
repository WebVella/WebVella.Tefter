namespace WebVella.Tefter.UI.Addons;

public partial class TucEmailViewColumnTypeEdit : TfLocalizedViewColumnComponent
{
	[Inject] protected IJSRuntime JsRuntime { get; set; } = null!;
	[Inject] protected IToastService ToastService { get; set; } = null!;
	[Parameter] public TfSpaceViewColumnEditModeContext Context { get; set; } = null!;
	[Parameter]
	public string? Value
	{
		get => _original;
		set
		{
			_original = value;
			_valueString = value?.ToString();
		}
	}

	[Parameter] public EventCallback<string> ValueChanged { get; set; }

	private readonly string _valueInputId = "input-" + Guid.NewGuid();

	private string? _original = null;
	private string? _valueString = null;


	private async Task _valueChanged()
	{
		if (!String.IsNullOrWhiteSpace(_valueString))
		{
			if (!_valueString!.IsValidEmail())
			{
				ToastService.ShowError(LOC("Invalid email format"));
				await InvokeAsync(StateHasChanged);
				await Task.Delay(100);
				_valueString = _original;
				await InvokeAsync(StateHasChanged);
				return;
			}
		}
		var settings = Context.GetSettings<TfEmailViewColumnTypeSettings>();
		if (!String.IsNullOrWhiteSpace(settings.ChangeConfirmationMessage)
		    && !await JsRuntime.InvokeAsync<bool>("confirm", settings.ChangeConfirmationMessage))
			return;

		Value = _valueString;
		await ValueChanged.InvokeAsync(Value);
		await JsRuntime.InvokeAsync<string>("Tefter.blurElementById", _valueInputId);
	}
}