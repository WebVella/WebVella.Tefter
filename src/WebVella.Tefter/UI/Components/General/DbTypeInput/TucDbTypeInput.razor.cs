using Microsoft.FluentUI.AspNetCore.Components;

namespace WebVella.Tefter.UI.Components;

public partial class TucDbTypeInput : TfBaseComponent
{
	[Parameter] public TfDatabaseColumnType DbType { get; set; } = TfDatabaseColumnType.Text;
	[Parameter] public object? Value { get; set; }
	[Parameter] public string? Placeholder { get; set; }
	[Parameter] public string? Class { get; set; }
	[Parameter] public string? Style { get; set; }
	[Parameter] public string? Error { get; set; }
	[Parameter] public EventCallback<object?> ValueChanged { get; set; }

	private string? _error = null;

	private async Task _valueChanged(object? value)
	{
		switch (DbType)
		{
			case TfDatabaseColumnType.ShortInteger:
				{
					await ValueChanged.InvokeAsync((short?)value);
				}
				return;
			case TfDatabaseColumnType.Integer:
				{
					await ValueChanged.InvokeAsync((int?)value);
				}
				return;
			case TfDatabaseColumnType.LongInteger:
				{
					await ValueChanged.InvokeAsync((long?)value);
				}
				return;
			case TfDatabaseColumnType.Number:
				{
					await ValueChanged.InvokeAsync((decimal?)value);
				}
				return;
			case TfDatabaseColumnType.Boolean:
				{
					bool? parsedValue = null;
					if (value is null) { }
					else if (value is string
						&& bool.TryParse((string)value, out bool outValue))
					{
						parsedValue = outValue;
					}
					await ValueChanged.InvokeAsync(parsedValue);
				}
				return;
			case TfDatabaseColumnType.DateOnly:
				{
					DateOnly? parsedValue = null;
					if (value is not null)
					{
						var dateTime = (DateTime)value;
						parsedValue = new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
					}
					await ValueChanged.InvokeAsync(parsedValue);
				}
				return;
			case TfDatabaseColumnType.DateTime:
				{
					await ValueChanged.InvokeAsync((DateTime?)value);
				}
				return;
			case TfDatabaseColumnType.ShortText:
				{
					await ValueChanged.InvokeAsync((string?)value);
				}
				return;
			case TfDatabaseColumnType.Text:
				{
					await ValueChanged.InvokeAsync((string?)value);
				}
				return;
			case TfDatabaseColumnType.Guid:
				{
					_error = null;
					Guid? parsedValue = null;
					if (value is null) { }
					else if (value is string)
					{
						if (Guid.TryParse((string)value, out Guid outGuid))
							parsedValue = outGuid;
						else
							_error = "invalid GUID";
					}
					await ValueChanged.InvokeAsync(parsedValue);
				}
				return;
			default:
				throw new Exception("DbType not supported");
		}
	}

	private string _getBoolLabel(bool? value)
	{
		if (value is null) return "NULL";
		return value.Value.ToString().ToUpperInvariant();
	}

	private string? _getError()
	{
		if(!String.IsNullOrWhiteSpace(_error)) return _error;
		if(!String.IsNullOrWhiteSpace(Error)) return Error;
		return null;
	}
}