namespace WebVella.Tefter.UI.Addons;
public partial class TucNumberViewColumnTypeOptions : TfLocalizedViewColumnComponent
{
	[Parameter] public TfSpaceViewColumnOptionsModeContext Context { get; set; } = null!;
	[Parameter] public EventCallback<TfNumberViewColumnTypeSettings> SettingsChanged { get; set; }
	
	private TfNumberViewColumnTypeSettings _form =  new ();

	protected override void OnParametersSet()
	{
		_form = Context.GetSettings<TfNumberViewColumnTypeSettings>();
	}	

	private async Task _onSettingsChanged(string propName, object? value)
	{
		switch (propName)
		{
			case nameof(_form.ChangeConfirmationMessage):
				_form.ChangeConfirmationMessage = (string?)value;
				break;			
			case nameof(_form.Format):
				_form.Format = (string?)value;
				break;				
		}
	
		await SettingsChanged.InvokeAsync(_form);
	}

}