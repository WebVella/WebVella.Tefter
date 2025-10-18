namespace WebVella.Tefter.UI.Addons;
public partial class TucShortIntegerViewColumnTypeOptions : TfLocalizedViewColumnComponent
{
	[Parameter] public TfShortIntegerViewColumnTypeSettings Settings { get; set; } = null!;
	[Parameter] public EventCallback<TfShortIntegerViewColumnTypeSettings> SettingsChanged { get; set; }
	[Parameter] public List<ValidationError> ValidationErrors { get; set; } = new();	
	

	private async Task _onSettingsChanged(string propName, object? value)
	{
		switch (propName)
		{
			case nameof(Settings.ChangeConfirmationMessage):
				Settings.ChangeConfirmationMessage = (string?)value;
				break;			
			case nameof(Settings.Format):
				Settings.Format = (string?)value;
				break;				
		}
	
		await SettingsChanged.InvokeAsync(Settings);
	}

}