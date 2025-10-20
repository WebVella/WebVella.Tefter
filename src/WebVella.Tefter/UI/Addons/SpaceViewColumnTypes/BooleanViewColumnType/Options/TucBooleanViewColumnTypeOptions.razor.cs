using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.UI.Addons;

public partial class TucBooleanViewColumnTypeOptions : TfLocalizedViewColumnComponent
{
	[Parameter] public TfSpaceViewColumnOptionsModeContext Context { get; set; } = null!;
	[Parameter] public EventCallback<TfBooleanViewColumnTypeSettings> SettingsChanged { get; set; }
	
	private TfBooleanViewColumnTypeSettings _settings =  new ();

	protected override void OnParametersSet()
	{
		_settings = Context.GetSettings<TfBooleanViewColumnTypeSettings>();
	}	

	private async Task _onSettingsChanged(string propName, object? value)
	{
		switch (propName)
		{
			case nameof(_settings.ShowLabel):
				_settings.ShowLabel = value is not null && (bool)value;
				break;				
			case nameof(_settings.TrueLabel):
				_settings.TrueLabel = (string?)value;
				break;				
			case nameof(_settings.FalseLabel):
				_settings.FalseLabel = (string?)value;
				break;			
			case nameof(_settings.NullLabel):
				_settings.NullLabel = (string?)value;
				break;				
			case nameof(_settings.TrueValueShowAsIcon):
				_settings.TrueValueShowAsIcon = value is not null && (bool)value;
				break;	
			case nameof(_settings.FalseValueShowAsIcon):
				_settings.FalseValueShowAsIcon = value is not null && (bool)value;
				break;	
			case nameof(_settings.NullValueShowAsIcon):
				_settings.NullValueShowAsIcon = value is not null && (bool)value;
				break;				
			case nameof(_settings.ChangeConfirmationMessage):
				_settings.ChangeConfirmationMessage = (string?)value;
				break;			
		}
	
		await SettingsChanged.InvokeAsync(_settings);
	}

}