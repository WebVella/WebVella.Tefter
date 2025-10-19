namespace WebVella.Tefter.UI.Addons;
public partial class TucSelectViewColumnTypeOptions : TfLocalizedViewColumnComponent
{
	[Parameter] public TfSelectViewColumnTypeSettings Settings { get; set; } = null!;
	[Parameter] public EventCallback<TfSelectViewColumnTypeSettings> SettingsChanged { get; set; }
	[Parameter] public List<ValidationError> ValidationErrors { get; set; } = new();	
	[Parameter] public TfDataset? SelectedDataset { get; set; } = null!;
	[Parameter] public List<TfDataset> DatasetOptions { get; set; } = null!;

	protected override void OnParametersSet()
	{
		Console.WriteLine($"OnParametersSet: {SelectedDataset?.Name}");
	}

	private async Task _datasetChangedAsync(TfDataset? dataset)
	{
		Settings.DatasetId = dataset?.Id;
		SelectedDataset = dataset;
		Console.WriteLine($"_datasetChangedAsync: {dataset?.Name}");
		await SettingsChanged.InvokeAsync(Settings);
	}

	private async Task _onSettingsChangedAsync(string propName, object? value)
	{
		switch (propName)
		{
			case nameof(Settings.ChangeConfirmationMessage):
				Settings.ChangeConfirmationMessage = (string?)value;
				break;			
			case nameof(Settings.Source):
				Settings.Source = (TfSelectViewColumnTypeSettingsSourceType)value!;
				break;			
			case nameof(Settings.OptionsString):
				Settings.OptionsString = (string?)value;
				break;			
			case nameof(Settings.SpaceDataValueColumnName):
				Settings.SpaceDataValueColumnName = (string?)value;
				break;			
			case nameof(Settings.SpaceDataLabelColumnName):
				Settings.SpaceDataLabelColumnName = (string?)value;
				break;	
			case nameof(Settings.SpaceDataColorColumnName):
				Settings.SpaceDataColorColumnName = (string?)value;
				break;		
			case nameof(Settings.SpaceDataBackgroundColorColumnName):
				Settings.SpaceDataBackgroundColorColumnName = (string?)value;
				break;		
			case nameof(Settings.SpaceDataIconColumnName):
				Settings.SpaceDataIconColumnName = (string?)value;
				break;				
		}
	
		await SettingsChanged.InvokeAsync(Settings);
	}

}