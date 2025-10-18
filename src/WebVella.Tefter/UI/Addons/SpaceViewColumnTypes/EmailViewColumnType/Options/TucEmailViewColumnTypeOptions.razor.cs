namespace WebVella.Tefter.UI.Addons;
public partial class TucEmailViewColumnTypeOptions : TfLocalizedViewColumnComponent
{
	[Parameter] public TfEmailViewColumnTypeSettings Settings { get; set; } = null!;
	[Parameter] public EventCallback<TfEmailViewColumnTypeSettings> SettingsChanged { get; set; }
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