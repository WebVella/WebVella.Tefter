﻿namespace WebVella.Tefter.Web.Addons;

/// <summary>
/// Description attribute is needed when presenting the component to the user as a select option
/// Localization attributes is needed to strongly type the location of the components translation resource
/// </summary>
[LocalizationResource("WebVella.Tefter.Web.Addons.SpaceViewColumnComponent.BooleanEditColumnComponent.TfBooleanEditColumnComponent", "WebVella.Tefter")]
public partial class TfBooleanEditColumnComponent : TucBaseViewColumn<TfBooleanEditColumnComponentOptions>
{
	#region << Constructor >>
	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	[ActivatorUtilitiesConstructor]
	public TfBooleanEditColumnComponent()
	{
	}

	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
	public TfBooleanEditColumnComponent(TfSpaceViewColumnScreenRegionContext context)
	{
		RegionContext = context;
	}
	#endregion

	#region << Properties >>
	public override Guid Id { get; init;} = new Guid("1faec857-0ae5-4912-ba6e-3a8624a06666");
	public override string Name { get; init;} = "Boolean Edit";
	public override string Description { get; init;} = String.Empty;
	public override string FluentIconName { get; init;} = String.Empty;
	public override List<Guid> SupportedColumnTypes { get; init;} = new List<Guid>{ 
		new Guid(TfConstants.TF_GENERIC_BOOLEAN_COLUMN_TYPE_ID),
	};
	/// <summary>
	/// The alias of the column name that stores the value.
	/// Depends on the ITfSpaceViewColumnType that renders this component
	/// by default it is 'Value'. The alias<>column name mapping is set by the user
	/// upon space view column configuration
	/// </summary>
	private string _valueAlias = "Value";
	private bool _isThreeState = false;
	private bool _value = false;
	private bool? _state = false;

	/// <summary>
	/// Each state has an unique hash and this is set in the component context under the Hash property value
	/// </summary>
	private Guid? _renderedHash = null;
	#endregion

	#region << Lifecycle >>
	/// <summary>
	/// When data needs to be inited, parameter set is the best place as Initialization is 
	/// done only once
	/// </summary>
	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		if (RegionContext.Hash != _renderedHash)
		{
			_initValues();
			_renderedHash = RegionContext.Hash;
		}
	}
	#endregion

	#region << Non rendered methods >>
	/// <summary>
	/// Overrides the default export method in order to apply its own options
	/// </summary>
	/// <returns></returns>
	public override void ProcessExcelCell(IServiceProvider serviceProvider,IXLCell excelCell)
	{
		object columnData = GetColumnDataByAlias(_valueAlias);
		if (columnData is not null && columnData is not bool) throw new Exception($"Not supported data type of '{columnData.GetType()}'. Supports Boolean.");
		bool? value = (bool?)columnData;
		if (value is null) return;

		//options are not inited yet as the component is not rendered
		var options = GetOptions();

		if (value.Value && !String.IsNullOrWhiteSpace(options.TrueLabel)) excelCell.SetValue(options.TrueLabel);
		else if (!value.Value && !String.IsNullOrWhiteSpace(options.FalseLabel)) excelCell.SetValue(options.FalseLabel);

		excelCell.SetValue(XLCellValue.FromObject(value));
	}
	#endregion

	#region << Private logic >>
	private async Task _onValueChange()
	{
		if (componentOptions.ChangeRequiresConfirmation)
		{
			var message = componentOptions.ChangeConfirmationMessage;
			if (String.IsNullOrWhiteSpace(message))
				message = LOC("Please confirm the data change!");

			if (!await JSRuntime.InvokeAsync<bool>("confirm", message))
			{
				await InvokeAsync(StateHasChanged);
				await Task.Delay(10);
				_initValues();
				await InvokeAsync(StateHasChanged);
				return;
			};
		}

		try
		{
			bool? value = _value;
			if (_isThreeState && _state is null) value = null;
			await OnRowColumnChangedByAlias(_valueAlias, value);
			ToastService.ShowSuccess(LOC("change applied"));
		}
		catch (Exception ex)
		{
			ToastService.ShowError(ex.Message);
			await InvokeAsync(StateHasChanged);
			await Task.Delay(10);
			_initValues();
			await InvokeAsync(StateHasChanged);
		}
	}

	private void _initValues()
	{
		var column = GetColumnInfoByAlias(_valueAlias);
		if (column is not null)
		{
			_isThreeState = column.IsNullable;
			object columnData = GetColumnDataByAlias(_valueAlias);
			if (columnData is not null && columnData is not bool) throw new Exception($"Not supported data type of '{columnData.GetType()}'");
			var value = (bool?)columnData;
			_value = value is null ? false : value.Value;
			_state = value;
		}
	}

	private string _getLabel()
	{
		if (!componentOptions.ShowLabel) return null;

		if (_isThreeState && _state is null)
		{
			if (!string.IsNullOrWhiteSpace(componentOptions.NullLabel)) return componentOptions.NullLabel;
			return "null";
		}
		else if (_value)
		{
			if (!string.IsNullOrWhiteSpace(componentOptions.TrueLabel)) return componentOptions.TrueLabel;
			return "true";
		}
		else
		{
			if (!string.IsNullOrWhiteSpace(componentOptions.FalseLabel)) return componentOptions.FalseLabel;
			return "false";
		}
	}
	#endregion
}

/// <summary>
/// The options used by the system to serialize and restore the components option
/// from the database
/// </summary>
public class TfBooleanEditColumnComponentOptions
{

	[JsonPropertyName("ShowLabel")]
	public bool ShowLabel { get; set; } = true;

	[JsonPropertyName("TrueLabel")]
	public string TrueLabel { get; set; }

	[JsonPropertyName("FalseLabel")]
	public string FalseLabel { get; set; }

	[JsonPropertyName("NullLabel")]
	public string NullLabel { get; set; }

	[JsonPropertyName("ChangeRequiresConfirmation")]
	public bool ChangeRequiresConfirmation { get; set; } = false;

	[JsonPropertyName("ChangeConfirmationMessage")]
	public string ChangeConfirmationMessage { get; set; }

}