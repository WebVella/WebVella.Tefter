namespace WebVella.Tefter.UI.Addons;
public partial class TucTextViewColumnTypeOptions : TfLocalizedViewColumnComponent
{
	[Parameter] public TfTextViewColumnTypeSettings Settings { get; set; } = null!;
	[Parameter] public EventCallback<TfTextViewColumnTypeSettings> SettingsChanged { get; set; }
	[Parameter] public List<ValidationError> ValidationErrors { get; set; } = new();	
	

	private async Task _onSettingsChanged(string propName, object? value)
	{
		switch (propName)
		{
			case nameof(Settings.ChangeConfirmationMessage):
				Settings.ChangeConfirmationMessage = (string?)value;
				break;			
		}
	
		await SettingsChanged.InvokeAsync(Settings);
	}

}