namespace WebVella.Tefter.UI.Components;
public partial class TucFilterQueryManage : TfBaseComponent
{

	[Parameter]
	public TfFilterQuery? Item { get; set; }

	[Parameter]
	public EventCallback<(string, List<string>?)> AddFilter { get; set; }

	[Parameter]
	public EventCallback<TfFilterQuery> RemoveFilter { get; set; }

	[Parameter]
	public EventCallback<TfFilterQuery> UpdateFilter { get; set; }

	[Parameter]
	public List<TfFilterQuery> AllOptions { get; set; } = new();

	[Parameter]
	public Dictionary<string, string> ColumnDict { get; set; } = new();

	[Parameter]
	public Dictionary<string, TfDatabaseColumnType> TypeDict { get; set; } = new();

	[Parameter]
	public bool Disabled { get; set; } = false;

	[Parameter]
	public bool ReadOnly { get; set; } = false;

	private TfFilterQuery _selectedOption = new();
	private string _columnViewTitle
	{
		get
		{
			if (Item is null) return "undefined";
			if (ColumnDict.ContainsKey(Item.Name))
				return ColumnDict[Item.Name];
			return Item.Name;
		}
	}

	private string _columnViewType
	{
		get
		{
			if (Item is null) return "undefined";
			if (TypeDict.ContainsKey(Item.Name))
				return TypeDict[Item.Name].ToDescriptionString();
			if (Item.Name.ToLowerInvariant() == "and"
			|| Item.Name.ToLowerInvariant() == "or")
				return "rule";
			return "undefined";
		}
	}

	private async Task _addColumnFilterHandler()
	{
		if (_selectedOption is null) return;
		if (Item is null) return;
		await AddFilter.InvokeAsync((_selectedOption.Name, Item.Path));
		_selectedOption = new(); //do not clear for convenience
	}

	private async Task _deleteFilterHandler()
	{
		await RemoveFilter.InvokeAsync(Item);
	}

	private async Task _valueChanged(TfDatabaseColumnType type, string propName, object? valueObj)
	{

		if (Item is null) return;
		if (Item.Name == new TfFilterAnd().GetColumnName()
			|| Item.Name == new TfFilterOr().GetColumnName()) return;

		TfFilterQuery updateObj = Item with { Name = Item.Name };

		if (type == TfDatabaseColumnType.Boolean)
		{
			var item = new TfFilterBoolean();
			if (propName == nameof(item.ComparisonMethod))
			{
				var value = (int)item.ComparisonMethod;
				if (valueObj is not null)
					value = (int)valueObj;

				if (updateObj.Method == value) return;

				updateObj.Method = value;
			}
			else if (propName == nameof(item.Value))
			{
				var value = (Option<string>?)valueObj;
				item.ValueOptionChanged(value);

				if (updateObj.Value == item.Value) return;

				updateObj.Value = item.Value;
			}
			else throw new Exception("propName not supported");
		}
		else if (type == TfDatabaseColumnType.DateOnly
			|| type == TfDatabaseColumnType.DateTime)
		{
			var item = new TfFilterDateTime();
			if (propName == nameof(item.ComparisonMethod))
			{
				var value = (int)item.ComparisonMethod;
				if (valueObj is not null)
					value = (int)valueObj;

				if (updateObj.Method == value) return;

				updateObj.Method = value;
			}
			else if (propName == nameof(item.Value))
			{
				item.ValueStringChanged((string?)valueObj);

				if (updateObj.Value == item.Value) return;

				updateObj.Value = item.Value;
			}
			else throw new Exception("propName not supported");
		}
		else if (type == TfDatabaseColumnType.Guid)
		{
			var item = new TfFilterGuid();
			if (propName == nameof(item.ComparisonMethod))
			{
				var value = (int)item.ComparisonMethod;
				if (valueObj is not null)
					value = (int)valueObj;

				if (updateObj.Method == value) return;

				updateObj.Method = value;
			}
			else if (propName == nameof(item.Value))
			{
				var value = (string?)valueObj;
				if (!String.IsNullOrWhiteSpace(value) && !Guid.TryParse(value, out Guid _))
					ToastService.ShowError(LOC("Invalid GUID value"));

				item.ValueStringChanged(value);

				if (updateObj.Value == item.Value) return;

				updateObj.Value = item.Value;
			}
			else throw new Exception("propName not supported");
		}
		else if (type == TfDatabaseColumnType.ShortInteger
			|| type == TfDatabaseColumnType.Integer
			|| type == TfDatabaseColumnType.LongInteger)
		{
			var item = new TfFilterNumeric();
			if (propName == nameof(item.ComparisonMethod))
			{
				var value = (int)item.ComparisonMethod;
				if (valueObj is not null)
					value = (int)valueObj;

				if (updateObj.Method == value) return;

				updateObj.Method = value;
			}
			else if (propName == nameof(item.Value))
			{
				item.ValueChanged((decimal?)(long?)valueObj);

				if (updateObj.Value == item.Value) return;

				updateObj.Value = item.Value;
			}
			else throw new Exception("propName not supported");
		}
		else if (type == TfDatabaseColumnType.Number)
		{
			var item = new TfFilterNumeric();
			if (propName == nameof(item.ComparisonMethod))
			{
				var value = (int)item.ComparisonMethod;
				if (valueObj is not null)
					value = (int)valueObj;

				if (updateObj.Method == value) return;

				updateObj.Method = value;
			}
			else if (propName == nameof(item.Value))
			{
				item.ValueChanged((decimal?)valueObj);

				if (updateObj.Value == item.Value) return;

				updateObj.Value = item.Value;
			}
			else throw new Exception("propName not supported");
		}
		else if (type == TfDatabaseColumnType.ShortText
		|| type == TfDatabaseColumnType.Text)
		{
			var item = new TfFilterText();
			if (propName == nameof(item.ComparisonMethod))
			{
				var value = (int)item.ComparisonMethod;
				if (valueObj is not null)
					value = (int)valueObj;

				if (updateObj.Method == value) return;

				updateObj.Method = value;
			}
			else if (propName == nameof(item.Value))
			{
				item.ValueChanged((string?)valueObj);

				if (updateObj.Value == item.Value) return;

				updateObj.Value = item.Value;
			}
			else throw new Exception("propName not supported");
		}
		else throw new Exception("Unsupported TucFilterBase in _valueChanged");

		await UpdateFilter.InvokeAsync(updateObj);
	}
}
