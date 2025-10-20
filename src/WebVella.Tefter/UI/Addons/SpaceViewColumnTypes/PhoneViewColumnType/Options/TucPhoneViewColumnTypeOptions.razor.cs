namespace WebVella.Tefter.UI.Addons;
public partial class TucPhoneViewColumnTypeOptions : TfLocalizedViewColumnComponent
{
	[Parameter] public TfSpaceViewColumnOptionsModeContext Context { get; set; } = null!;
	[Parameter] public EventCallback<TfPhoneViewColumnTypeSettings> SettingsChanged { get; set; }
	
	private TfPhoneViewColumnTypeSettings _form =  new ();

	protected override void OnParametersSet()
	{
		_form = Context.GetSettings<TfPhoneViewColumnTypeSettings>();
	}	
	

	private async Task _onSettingsChanged(string propName, object? value)
	{
		switch (propName)
		{
			case nameof(_form.ChangeConfirmationMessage):
				_form.ChangeConfirmationMessage = (string?)value;
				break;			
		}
	
		await SettingsChanged.InvokeAsync(_form);
	}

}