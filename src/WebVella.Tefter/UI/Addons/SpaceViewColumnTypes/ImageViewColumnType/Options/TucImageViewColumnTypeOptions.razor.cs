namespace WebVella.Tefter.UI.Addons;
public partial class TucImageViewColumnTypeOptions : TfLocalizedViewColumnComponent
{
	[Parameter] public TfSpaceViewColumnOptionsMode Context { get; set; } = null!;
	[Parameter] public EventCallback<TfImageViewColumnTypeSettings> SettingsChanged { get; set; }
	
	private TfImageViewColumnTypeSettings _form =  new ();

	protected override void OnParametersSet()
	{
		_form = Context.GetSettings<TfImageViewColumnTypeSettings>();
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