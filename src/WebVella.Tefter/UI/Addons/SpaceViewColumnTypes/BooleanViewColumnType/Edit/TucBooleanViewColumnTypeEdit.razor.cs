namespace WebVella.Tefter.UI.Addons;

public partial class TucBooleanViewColumnTypeEdit : TfLocalizedViewColumnComponent
{
	[Inject] protected IJSRuntime JSRuntime { get; set; } = null!;
	[Parameter] public bool? Value { get; set; }
	[Parameter] public EventCallback<bool?> ValueChanged { get; set; }
	[Parameter] public TfBooleanViewColumnTypeSettings Settings { get; set; } = null!;
	[Parameter] public bool IsNullableColumn { get; set; } = false;

	private readonly string _valueInputId = "input-" + Guid.NewGuid();


	private async Task _valueChanged(bool? value)
	{
		if (!String.IsNullOrWhiteSpace(Settings.ChangeConfirmationMessage)
		    && !await JSRuntime.InvokeAsync<bool>("confirm", Settings.ChangeConfirmationMessage))
			return;
		Value = value;
		await ValueChanged.InvokeAsync(Value);
		await JSRuntime.InvokeAsync<string>("Tefter.blurElementById", _valueInputId);
	}

	private async Task _valueChangedNotNull(bool value)
	{
		if (!String.IsNullOrWhiteSpace(Settings.ChangeConfirmationMessage)
		    && !await JSRuntime.InvokeAsync<bool>("confirm", Settings.ChangeConfirmationMessage))
			return;
		Value = value;
		await ValueChanged.InvokeAsync(Value);
		await JSRuntime.InvokeAsync<string>("Tefter.blurElementById", _valueInputId);
	}

	private string? _getLabel()
	{
		if (!Settings.ShowLabel) return null;

		if (IsNullableColumn && Value is null)
		{
			if (!string.IsNullOrWhiteSpace(Settings.NullLabel)) return Settings.NullLabel;
			return "null";
		}
		else if (Value is null || !Value.Value)
		{
			if (!string.IsNullOrWhiteSpace(Settings.FalseLabel)) return Settings.FalseLabel;
			return "false";
		}
		else
		{
			if (!string.IsNullOrWhiteSpace(Settings.TrueLabel)) return Settings.TrueLabel;
			return "true";
		}
	}
}