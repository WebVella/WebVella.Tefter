namespace WebVella.Tefter.UI.Addons;
public partial class TucUrlViewColumnTypeOptions : TfLocalizedViewColumnComponent
{
	[Parameter] public TfUrlViewColumnTypeSettings Settings { get; set; } = null!;
	[Parameter] public EventCallback<TfUrlViewColumnTypeSettings> SettingsChanged { get; set; }
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