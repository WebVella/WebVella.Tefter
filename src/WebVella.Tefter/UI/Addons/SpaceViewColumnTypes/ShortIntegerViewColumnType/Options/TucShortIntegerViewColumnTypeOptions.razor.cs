namespace WebVella.Tefter.UI.Addons;
public partial class TucShortIntegerViewColumnTypeOptions : TfLocalizedViewColumnComponent
{
	[Parameter] public TfSpaceViewColumnOptionsMode Context { get; set; } = null!;
	[Parameter] public EventCallback<TfShortIntegerViewColumnTypeSettings> SettingsChanged { get; set; }
	
	private TfShortIntegerViewColumnTypeSettings _form =  new ();

	protected override void OnParametersSet()
	{
		_form = Context.GetSettings<TfShortIntegerViewColumnTypeSettings>();
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