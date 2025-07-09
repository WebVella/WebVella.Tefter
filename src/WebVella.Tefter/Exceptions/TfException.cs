using WebVella.Tefter.UI.Components;

namespace WebVella.Tefter.Exceptions;

public class TfException : Exception
{
	public TfException() : base(message: string.Empty) { }

	public TfException(string message) : base(message) { }

	public TfException(string message, Exception innerException) : base(message, innerException)
	{
		if (innerException != null && innerException.Data.Count > 0)
			AddData(innerException.Data);
	}

	public TfException(string message, System.Collections.IDictionary dictionary) : base(message)
	{
		AddData(dictionary);
	}

	public TfException(string message, Dictionary<string, List<string>> dictionary) : base(message)
	{
		AddData(dictionary);
	}

	public void UpsertDataList(string key, string value, long index = -1)
	{
		if (!Data.Contains(key))
			Data.Add(key, new List<ValidationError>());

		(Data[key] as List<ValidationError>)?.Add(new ValidationError(key,value,null,index));
	}

	public void ThrowIfContainsErrors()
	{
		if (Data.Count > 0)
			throw this;
	}

	public void AddData(IDictionary dictionary)
	{
		if (dictionary != null)
		{
			foreach (DictionaryEntry item in dictionary)
				Data.Add(item.Key, item.Value);
		}
	}

	public void AddData(Dictionary<string, List<string>> dictionary)
	{
		if (dictionary != null)
		{
			foreach (var item in dictionary)
				Data.Add(item.Key, item.Value);
		}
	}

	public IDictionary<string, List<ValidationError>> GetDataAsUsableDictionary()
	{
		IDictionary<string, List<ValidationError>> dict = new Dictionary<string, List<ValidationError>>();
		foreach (var key in Data.Keys)
		{
			dict.Add(key.ToString(), (Data[key] as List<ValidationError>) ?? new List<ValidationError>());
		}
		return dict;
	}

	public List<ValidationError> GetDataAsValidationErrorList()
	{
		List<ValidationError> result = new List<ValidationError>();
		foreach (var key in Data.Keys)
		{
			result.AddRange((Data[key] as List<ValidationError>) ?? new List<ValidationError>());
		}
		return result;
	}
}
