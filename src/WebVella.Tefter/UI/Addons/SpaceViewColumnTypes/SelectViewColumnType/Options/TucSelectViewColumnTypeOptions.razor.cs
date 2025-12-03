namespace WebVella.Tefter.UI.Addons;
public partial class TucSelectViewColumnTypeOptions : TfLocalizedViewColumnComponent
{
	[Parameter] public TfSpaceViewColumnOptionsMode Context { get; set; } = null!;
	[Parameter] public EventCallback<TfSelectViewColumnTypeSettings> SettingsChanged { get; set; }
	[Parameter] public TfDataset? SelectedDataset { get; set; } = null!;
	[Parameter] public List<TfDataset> DatasetOptions { get; set; } = null!;

	private TfSelectViewColumnTypeSettings _form =  new ();

	protected override void OnParametersSet()
	{
		_form = Context.GetSettings<TfSelectViewColumnTypeSettings>();
	}	
	
	private async Task _datasetChangedAsync(TfDataset? dataset)
	{
		_form.DatasetId = dataset?.Id;
		SelectedDataset = dataset;
		await SettingsChanged.InvokeAsync(_form);
	}

	private async Task _onSettingsChangedAsync(string propName, object? value)
	{
		switch (propName)
		{
			case nameof(_form.ChangeConfirmationMessage):
				_form.ChangeConfirmationMessage = (string?)value;
				break;			
			case nameof(_form.Source):
				_form.Source = (TfSelectViewColumnTypeSettingsSourceType)value!;
				break;			
			case nameof(_form.OptionsString):
				_form.OptionsString = (string?)value;
				break;			
			case nameof(_form.SpaceDataValueColumnName):
				_form.SpaceDataValueColumnName = (string?)value;
				break;			
			case nameof(_form.SpaceDataLabelColumnName):
				_form.SpaceDataLabelColumnName = (string?)value;
				break;	
			case nameof(_form.SpaceDataColorColumnName):
				_form.SpaceDataColorColumnName = (string?)value;
				break;		
			case nameof(_form.SpaceDataBackgroundColorColumnName):
				_form.SpaceDataBackgroundColorColumnName = (string?)value;
				break;		
			case nameof(_form.SpaceDataIconColumnName):
				_form.SpaceDataIconColumnName = (string?)value;
				break;				
		}
	
		await SettingsChanged.InvokeAsync(_form);
	}

}