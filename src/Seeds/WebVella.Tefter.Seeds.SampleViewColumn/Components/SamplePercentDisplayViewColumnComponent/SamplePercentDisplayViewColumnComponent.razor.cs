namespace WebVella.Tefter.Seeds.SampleViewColumn.Components;

[Description("Percent Display")]
public partial class SamplePercentDisplayViewColumnComponent : TucBaseViewColumn<SamplePercentDisplayViewColumnComponentOptions>
{
	#region << Constructor >>
	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	[ActivatorUtilitiesConstructor]
	public SamplePercentDisplayViewColumnComponent()
	{
	}

	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>	
	public SamplePercentDisplayViewColumnComponent(TucViewColumnComponentContext context)
	{
		Context = context;
	}
	#endregion

	#region << Properties >>
	public override Guid Id { get; init; } = new Guid("27604066-f558-4a04-9278-c7b3e8e97c50");
	public override List<Type> SupportedColumnTypes { get; init; } = new List<Type> {
		typeof(SamplePercentViewColumnType) };
	/// <summary>
	/// The alias of the column name that stores the value.
	/// Depends on the ITfSpaceViewColumnType that renders this component
	/// by default it is 'Value'. The alias<>column name mapping is set by the user
	/// upon space view column configuration
	/// </summary>
	private string _valueAlias = "Value";
	private decimal? _value = null;

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
		if (Context.Hash != _renderedHash)
		{
			_initValues();
			_renderedHash = Context.Hash;
		}
	}
	#endregion

	private void _initValues()
	{
		object columnData = GetColumnDataByAlias(_valueAlias);
		if (columnData is not null && !(columnData is int || columnData is short || columnData is long || columnData is decimal))
			throw new Exception($"Not supported data type of '{columnData.GetType()}'. Supports only number based types.");
		
		var settings = JsonSerializer.Deserialize<SamplePercentDisplayViewColumnComponentOptions>(Context.CustomOptionsJson); 

		if(columnData is null)
		{
			_value = null;
			return;
		}

		_value = ( Convert.ToDecimal(columnData) / settings.MaxValue ) * 100;
	}
}

/// <summary>
/// The options used by the system to serialize and restore the components option
/// from the database
/// </summary>
public class SamplePercentDisplayViewColumnComponentOptions
{
	[JsonPropertyName("MaxValue")]
	public decimal MaxValue { get; set; } = 1;
}