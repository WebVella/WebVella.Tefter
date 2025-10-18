namespace WebVella.Tefter.UI.Addons;
public partial class TucPhoneViewColumnTypeOptions : TfLocalizedViewColumnComponent
{
	[Parameter] public TfPhoneViewColumnTypeSettings Settings { get; set; } = null!;
	[Parameter] public EventCallback<TfPhoneViewColumnTypeSettings> SettingsChanged { get; set; }
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