using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.UI.Addons;

public partial class TucTextViewColumnTypeOptions : TfLocalizedViewColumnComponent,IDisposable
{
	[Parameter] public TfTextViewColumnTypeSettings Settings { get; set; } = null!;
	[Parameter] public EditContext? EditContext { get; set; } = null;
	[Parameter] public ValidationMessageStore? ValidationMessageStore { get; set; } = null;
	[Parameter] public EventCallback<TfTextViewColumnTypeSettings> OptionsChanged { get; set; }	
	
	public void Dispose()
	{
		if (EditContext is not null)
		{
			EditContext.OnValidationRequested -= OnOptionsValidationRequested;
		}
	}	
	
	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (EditContext is not null)
			EditContext.OnValidationRequested += OnOptionsValidationRequested;
	}	
	
	private async Task _onOptionsChanged(string propName, object? value)
	{
		switch (propName)
		{
			case nameof(Settings.ChangeConfirmationMessage):
				Settings.ChangeConfirmationMessage = (string?)value;
				break;			
		}
	
		await OptionsChanged.InvokeAsync(Settings);
	}
	
	private void OnOptionsValidationRequested(object? sender, ValidationRequestedEventArgs e)
	{
		//nothing to validate
	}	

}