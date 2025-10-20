namespace WebVella.Tefter.UI.Addons;
public partial class TucGuidViewColumnTypeOptions : TfLocalizedViewColumnComponent
{
	[Parameter] public TfSpaceViewColumnOptionsModeContext Context { get; set; } = null!;
	[Parameter] public EventCallback<TfGuidViewColumnTypeSettings> SettingsChanged { get; set; }

	private TfGuidViewColumnTypeSettings _form =  new ();
	protected override void OnParametersSet()
	{
		_form = Context.GetSettings<TfGuidViewColumnTypeSettings>();
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