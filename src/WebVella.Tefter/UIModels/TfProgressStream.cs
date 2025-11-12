using System.Threading.Channels;

namespace WebVella.Tefter.Models;

public class TfProgressStream
{
	private readonly Channel<TfProgressStreamItem> _channel;
	private List<TfProgressStreamItem> _items = new();

	
	public event Action<TfProgressStreamItem> OnProgress = null!;

	public TfProgressStream()
	{
		_channel = Channel.CreateBounded<TfProgressStreamItem>(200);

		// Start listening to the channel
		Task.Run(async () =>
		{
			await foreach (var message in _channel.Reader.ReadAllAsync())
			{
				OnProgress?.Invoke(message);
			}
		});
	}

	public void ReportProgress(TfProgressStreamItem message)
	{
		_channel.Writer.TryWrite(message);
		_items.Add(message);
	}
	
	public void ReportProgress(List<TfProgressStreamItem> messages)
	{
		foreach (var message in messages)
		{
			_channel.Writer.TryWrite(message);
			_items.Add(message);
		}
	}

	public ReadOnlyCollection<TfProgressStreamItem> GetProgressLog()
		=> _items.AsReadOnly();
}

public record TfProgressStreamItem
{
	//for presentation purposes
	public string Id { get; set; } = $"tf-{Guid.NewGuid()}";
	public DateTime Timestamp { get; set; } = DateTime.Now;
	public string Message { get; set; } = "";
	public TfProgressStreamItemType Type { get; set; } = TfProgressStreamItemType.Normal;

	public TfColor? Color
	{
		get
		{
			if (Type == TfProgressStreamItemType.Error)
				return TfColor.Red500;
			if (Type == TfProgressStreamItemType.Warning)
				return TfColor.Orange500;
			if (Type == TfProgressStreamItemType.Success)
				return TfColor.Green500;
			if (Type == TfProgressStreamItemType.Debug)
				return TfColor.Gray500;			
			return null;
		}
	}
}

public enum TfProgressStreamItemType
{
	Normal = 0,
	Warning = 1,
	Error = 2,
	Success = 3,
	Debug = 4,
}