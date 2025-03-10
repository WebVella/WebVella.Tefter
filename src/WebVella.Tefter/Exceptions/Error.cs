using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.Tefter.Exceptions;

public interface IError
{
	string Message { get; }
	Dictionary<string, object> Metadata { get; }
	List<IError> Reasons { get; }
}

public class Error : IError
{
	public string Message { get; protected set; }

	public Dictionary<string, object> Metadata { get; }

	public List<IError> Reasons { get; }

	protected Error()
	{
		Metadata = new Dictionary<string, object>();
		Reasons = new List<IError>();
	}

	public Error(string message)
		: this()
	{
		Message = message;
	}

	public Error(string message, IError causedBy)
		: this(message)
	{
		if (causedBy == null)
			throw new ArgumentNullException(nameof(causedBy));

		Reasons.Add(causedBy);
	}

	public Error WithMetadata(string metadataName, object metadataValue)
	{
		Metadata.Add(metadataName, metadataValue);
		return this;
	}

	public Error WithMetadata(Dictionary<string, object> metadata)
	{
		foreach (var metadataItem in metadata)
		{
			Metadata.Add(metadataItem.Key, metadataItem.Value);
		}

		return this;
	}

	public override string ToString()
	{
		return Message;
	}
}