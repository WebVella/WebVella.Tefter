namespace WebVella.Tefter.UI.Addons;
public partial class TucEmailViewColumnTypeOptions : TfLocalizedViewColumnComponent
{
	[Parameter] public TfSpaceViewColumnOptionsMode Context { get; set; } = null!;
	[Parameter] public EventCallback<TfEmailViewColumnTypeSettings> SettingsChanged { get; set; }
	private TfEmailViewColumnTypeSettings _form =  new ();

	protected override void OnParametersSet()
	{
		_form = Context.GetSettings<TfEmailViewColumnTypeSettings>();
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