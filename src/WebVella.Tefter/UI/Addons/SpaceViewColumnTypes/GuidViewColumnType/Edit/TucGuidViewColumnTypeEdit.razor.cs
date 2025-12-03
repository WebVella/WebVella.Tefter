namespace WebVella.Tefter.UI.Addons;

public partial class TucGuidViewColumnTypeEdit : TfLocalizedViewColumnComponent
{
	[Inject] protected IJSRuntime JsRuntime { get; set; } = null!;
	[Inject] protected IToastService ToastService { get; set; } = null!;
	[Parameter] public TfSpaceViewColumnEditMode Context { get; set; } = null!;
	[Parameter]
	public Guid? Value
	{
		get => _original;
		set
		{
			_original = value;
			_valueString = value?.ToString();
		}
	}

	[Parameter] public EventCallback<Guid?> ValueChanged { get; set; }

	private readonly string _valueInputId = "input-" + Guid.NewGuid();

	private Guid? _original = null;
	private string? _valueString = null;

	private async Task _valueChanged()
	{
		Guid? value = null;
		if (!String.IsNullOrWhiteSpace(_valueString))
		{
			if (Guid.TryParse(_valueString, out Guid outGuid))
			{
				value = outGuid;
			}
			else
			{
				ToastService.ShowError(LOC("Invalid GUID format"));
				await InvokeAsync(StateHasChanged);
				await Task.Delay(100);
				_valueString = _original?.ToString(); 
				await InvokeAsync(StateHasChanged);
				return;
			}
		}
		var settings = Context.GetSettings<TfGuidViewColumnTypeSettings>();
		if (!String.IsNullOrWhiteSpace(settings.ChangeConfirmationMessage)
		    && !await JsRuntime.InvokeAsync<bool>("confirm", settings.ChangeConfirmationMessage))
			return;

		Value = value;
		await ValueChanged.InvokeAsync(Value);
		await JsRuntime.InvokeAsync<string>("Tefter.blurElementById", _valueInputId);
	}
}