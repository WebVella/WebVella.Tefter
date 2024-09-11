namespace WebVella.Tefter;

public class TfDataRow : IEnumerable
{
	private readonly object[] _values;

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
