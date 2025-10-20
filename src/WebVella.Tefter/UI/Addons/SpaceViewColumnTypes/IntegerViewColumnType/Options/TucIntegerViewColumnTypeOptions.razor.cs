namespace WebVella.Tefter.UI.Addons;
public partial class TucIntegerViewColumnTypeOptions : TfLocalizedViewColumnComponent
{
	[Parameter] public TfSpaceViewColumnOptionsModeContext Context { get; set; } = null!;
	[Parameter] public EventCallback<TfIntegerViewColumnTypeSettings> SettingsChanged { get; set; }
	
	private TfIntegerViewColumnTypeSettings _form =  new ();

	protected override void OnParametersSet()
	{
		_form = Context.GetSettings<TfIntegerViewColumnTypeSettings>();
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