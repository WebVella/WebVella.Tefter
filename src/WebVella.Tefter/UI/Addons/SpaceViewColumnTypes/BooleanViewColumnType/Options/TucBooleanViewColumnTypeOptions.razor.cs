using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.UI.Addons;

public partial class TucBooleanViewColumnTypeOptions : TfLocalizedViewColumnComponent
{
	[Parameter] public TfBooleanViewColumnTypeSettings Settings { get; set; } = null!;
	[Parameter] public EventCallback<TfBooleanViewColumnTypeSettings> SettingsChanged { get; set; }
	[Parameter] public List<ValidationError> ValidationErrors { get; set; } = new();	
	

	private async Task _onSettingsChanged(string propName, object? value)
	{
		switch (propName)
		{
			case nameof(Settings.ShowLabel):
				Settings.ShowLabel = value is not null && (bool)value;
				break;				
			case nameof(Settings.TrueLabel):
				Settings.TrueLabel = (string?)value;
				break;				
			case nameof(Settings.FalseLabel):
				Settings.FalseLabel = (string?)value;
				break;			
			case nameof(Settings.NullLabel):
				Settings.NullLabel = (string?)value;
				break;				
			case nameof(Settings.TrueValueShowAsIcon):
				Settings.TrueValueShowAsIcon = value is not null && (bool)value;
				break;	
			case nameof(Settings.FalseValueShowAsIcon):
				Settings.FalseValueShowAsIcon = value is not null && (bool)value;
				break;	
			case nameof(Settings.NullValueShowAsIcon):
				Settings.NullValueShowAsIcon = value is not null && (bool)value;
				break;				
			case nameof(Settings.ChangeConfirmationMessage):
				Settings.ChangeConfirmationMessage = (string?)value;
				break;			
		}
	
		await SettingsChanged.InvokeAsync(Settings);
	}

}