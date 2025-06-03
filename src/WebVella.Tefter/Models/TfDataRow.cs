using Microsoft.FluentUI.AspNetCore.Components;

namespace WebVella.Tefter;

public class TfDataRow : IEnumerable
{
	private readonly object[] _values;
	internal object[] Values => _values;
	public TfDataTable DataTable { get; init; }

	public object this[int columnIndex]
	{
		get { return _values[columnIndex]; }
	}

	public object this[string columnName]
	{
		get
		{
			if (string.IsNullOrWhiteSpace(columnName))
				throw new ArgumentException(nameof(columnName));

			int index = DataTable.Columns.IndexOf(x => x.Name == columnName);
			if (index == -1)
				return null;
			//throw new Exception($"A column with name {columnName} is not found in DataTable object.");

			return _values[index];
		}
		set
		{
			if (string.IsNullOrWhiteSpace(columnName))
				throw new ArgumentException(nameof(columnName));

			int index = DataTable.Columns.IndexOf(x => x.Name == columnName);
			if (index == -1)
				throw new Exception($"A column with name {columnName} is not found in DataTable object.");

			var column = DataTable.Columns[columnName];

			if (!column.IsNullable && value is null)
				throw new Exception($"Trying to set null as value to non nullable column.");

			if (column.IsNullable && value is null)
			{
				_values[index] = value;
				return;
			}

			if (column.IsJoinColumn)
			{
				switch (column.DbType)
				{
					case TfDatabaseColumnType.Guid:
						if (value is not List<Guid?>)
							throw new Exception($"Trying to set non Guid value as value to Guid column.");
						break;
					case TfDatabaseColumnType.Boolean:
						if (value is not List<bool?>)
							throw new Exception($"Trying to set non boolean value as value to boolean column.");
						break;
					case TfDatabaseColumnType.DateOnly:
						if (value is not List<DateOnly?>)
							throw new Exception($"Trying to set non DateOnly value as value to Date column.");
						break;
					case TfDatabaseColumnType.DateTime:
						if (value is not List<DateTime?>)
							throw new Exception($"Trying to set non DateTime value as value to DateTime column.");
						break;
					case TfDatabaseColumnType.ShortInteger:
						if (value is not List<short?>)
							throw new Exception($"Trying to set non short value as value to ShortInteger column.");
						break;
					case TfDatabaseColumnType.Integer:
						if (value is not List<int?>)
							throw new Exception($"Trying to set non integer value as value to Integer column.");
						break;
					case TfDatabaseColumnType.LongInteger:
						if (value is not List<long?>)
							throw new Exception($"Trying to set non long integer value as value to LongInteger column.");
						break;
					case TfDatabaseColumnType.Number:
						if (value is not List<decimal?>)
							throw new Exception($"Trying to set non number value as value to Number column.");
						break;
					case TfDatabaseColumnType.ShortText:
					case TfDatabaseColumnType.Text:
						if (value is not List<string>)
							throw new Exception($"Trying to set non text value as value to Text or ShortText column.");
						break;
					case TfDatabaseColumnType.AutoIncrement:
						throw new Exception($"Trying to set value to autoincrement column.Not allowed.");
					default:
						throw new Exception($"Not supported database column type.");
				}
			}
			else
			{
				switch (column.DbType)
				{
					case TfDatabaseColumnType.Guid:
						if (value is not Guid)
							throw new Exception($"Trying to set non Guid value as value to Guid column.");
						break;
					case TfDatabaseColumnType.Boolean:
						if (value is not bool)
							throw new Exception($"Trying to set non boolean value as value to boolean column.");
						break;
					case TfDatabaseColumnType.DateOnly:
						if (value is not DateOnly)
							throw new Exception($"Trying to set non DateOnly value as value to Date column.");
						break;
					case TfDatabaseColumnType.DateTime:
						if (value is not DateTime)
							throw new Exception($"Trying to set non DateTime value as value to DateTime column.");
						break;
					case TfDatabaseColumnType.ShortInteger:
						if (value is not short)
							throw new Exception($"Trying to set non short value as value to ShortInteger column.");
						break;
					case TfDatabaseColumnType.Integer:
						if (value is not int)
							throw new Exception($"Trying to set non integer value as value to Integer column.");
						break;
					case TfDatabaseColumnType.LongInteger:
						if (value is not long)
							throw new Exception($"Trying to set non long integer value as value to LongInteger column.");
						break;
					case TfDatabaseColumnType.Number:
						if (value is not decimal)
							throw new Exception($"Trying to set non number value as value to Number column.");
						break;
					case TfDatabaseColumnType.ShortText:
					case TfDatabaseColumnType.Text:
						if (value is not string)
							throw new Exception($"Trying to set non text value as value to Text or ShortText column.");
						break;
					case TfDatabaseColumnType.AutoIncrement:
						throw new Exception($"Trying to set value to autoincrement column.Not allowed.");
					default:
						throw new Exception($"Not supported database column type.");
				}
			}

			_values[index] = value;
		}
	}

	public TfDataRow(TfDataTable dataTable, object[] values)
	{
		_values = values;
		this.DataTable = dataTable;
	}

	public T Field<T>(string columnName)
	{
		return UnboxT<T>.Unbox(this[columnName]);
	}

	public IEnumerator GetEnumerator()
	{
		return _values.GetEnumerator();
	}

	internal object[] GetValues()
	{
		object[] values = new object[_values.Length];
		Array.Copy(_values, values, values.Length);
		return values;
	}

	public Guid? GetJoinKeyValue(string joinKeyName)
	{
		if (string.IsNullOrEmpty(joinKeyName))
			return null;

		string skColumnName = $"tf_jk_{joinKeyName}_id";

		int index = DataTable.Columns.IndexOf(x => x.Name == skColumnName);
		if (index == -1)
			return null;

		return (Guid)_values[index];
	}

	public string GetDataIdentityValue(string dataIdentity )
	{
		if (string.IsNullOrEmpty(dataIdentity))
			return null;

		var dataIdentityColumn = $"tf_ide_{dataIdentity}";
		if(dataIdentity == TfConstants.TF_ROW_ID_DATA_IDENTITY)
			dataIdentityColumn = "tf_row_id";

		int index = DataTable.Columns.IndexOf(x => x.Name == dataIdentityColumn);
		if (index == -1)
			return null;

		return (string)_values[index];
	}

	private static class UnboxT<T>
	{
		internal static readonly Converter<object, T> Unbox = Create(typeof(T));

		private static Converter<object, T> Create(Type type)
		{
			if (type.IsValueType)
			{
				if (type.IsGenericType && !type.IsGenericTypeDefinition && (typeof(Nullable<>) == type.GetGenericTypeDefinition()))
				{
					return (Converter<object, T>)Delegate.CreateDelegate(
						typeof(Converter<object, T>),
							typeof(UnboxT<T>)
								.GetMethod("NullableField", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
								.MakeGenericMethod(type.GetGenericArguments()[0]));
				}
				return ValueField;
			}
			return ReferenceField;
		}

		private static T ReferenceField(object value)
		{
			return ((DBNull.Value == value) ? default(T) : (T)value);
		}

		private static T ValueField(object value)
		{
			return (T)value;
		}

		private static Nullable<TElem> NullableField<TElem>(object value) where TElem : struct
		{
			if (value is null)
				return default(Nullable<TElem>);

			return new Nullable<TElem>((TElem)value);
		}
	}

}
