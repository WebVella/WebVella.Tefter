using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.Web.ViewColumns;

/// <summary>
/// Description attribute is needed when presenting the component to the user as a select option
/// Localization attributes is needed to strongly type the location of the components translation resource
/// </summary>
[Description("Tefter Text Edit")]
[LocalizationResource("WebVella.Tefter.Web.ViewColumns.Components.TextEditColumnComponent.TfTextEditColumnComponent", "WebVella.Tefter")]
public partial class TfTextEditColumnComponent : TfBaseViewColumn<TfTextEditColumnComponentOptions>
{
	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	public TfTextEditColumnComponent()
	{
	}


	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
	public TfTextEditColumnComponent(TfComponentContext context)
	{
		Context = context;
	}
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_value = GetDataObjectByAlias(_valueAlias);
	}

	protected override void OnValidationRequested(object sender, ValidationRequestedEventArgs e)
	{
		base.OnValidationRequested(sender, e);
		//Context.ValidationMessageStore.Add(Context.EditContext.Field(nameof(TucSpaceViewColumn.CustomOptionsJson)), "problem with json");

	}

	/// <summary>
	/// The alias of the column name that stores the value.
	/// Depends on the ITfSpaceViewColumnType that renders this component
	/// by default it is 'Value'. The alias<>column name mapping is set by the user
	/// upon space view column configuration
	/// </summary>
	private string _valueAlias = "Value";
	private string _value = null;

	/// <summary>
	/// Overrides the default export method in order to apply its own options
	/// </summary>
	/// <returns></returns>
	public override object GetData()
	{
		return GetDataObjectByAlias(_valueAlias);
	}

	private async Task _valueChanged()
	{
		if (!RowChanged.HasDelegate) return;
		try
		{
			var dt = Context.DataTable.NewTable(Context.RowIndex);
			if (dt.Rows.Count == 0)
			{
				ToastService.ShowError(LOC("Row with index {0} is not found", Context.RowIndex));
				return;
			}
			var colName = GetColumnNameFromAlias(_valueAlias);
			if (String.IsNullOrWhiteSpace(colName))
			{
				ToastService.ShowError(LOC("Column for the alias {0} is not found", _valueAlias));
				return;
			}
			dt.Rows[0][colName] = _value;

			await OnRowChanged(dt);
		}
		catch(Exception ex){ 
			ToastService.ShowError(ex.Message);
			await InvokeAsync(StateHasChanged);
			await Task.Delay(10);
			_value = GetDataObjectByAlias(_valueAlias);
			await InvokeAsync(StateHasChanged);
		}
	}
}

public class TfTextEditColumnComponentOptions
{

}