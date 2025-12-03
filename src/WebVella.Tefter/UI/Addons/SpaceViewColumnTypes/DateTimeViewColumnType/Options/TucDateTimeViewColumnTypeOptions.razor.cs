namespace WebVella.Tefter.UI.Addons;
public partial class TucDateTimeViewColumnTypeOptions : TfLocalizedViewColumnComponent
{
	[Parameter] public TfSpaceViewColumnOptionsMode Context { get; set; } = null!;
	[Parameter] public EventCallback<TfDateTimeViewColumnTypeSettings> SettingsChanged { get; set; }
	private TfDateTimeViewColumnTypeSettings _form =  new ();

	protected override void OnParametersSet()
	{
		_form = Context.GetSettings<TfDateTimeViewColumnTypeSettings>();
	}
	

	private async Task _onSettingsChanged(string propName, object? value)
	{
		switch (propName)
		{
			case nameof(_form.ChangeConfirmationMessage):
				_form.ChangeConfirmationMessage = (string?)value;
				break;			
			case nameof(_form.CalendarViewsSelection):
				_form.CalendarViewsSelection = (CalendarViews)value!;
				break;				
			case nameof(_form.Format):
				_form.Format = (string?)value;
				break;				
		}
	
		await SettingsChanged.InvokeAsync(_form);
	}

}