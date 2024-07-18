//using System.Threading.Channels;

//namespace WebVella.Tefter;

//public class AsyncDataTable
//{
//	private readonly Channel<AsyncDataRow> _channel;
//	private readonly List<string> _columns;
//	private readonly NpgsqlDataReader _sqlReader;

//	public ReadOnlyCollection<string> Columns => _columns.AsReadOnly();

//	public AsyncDataTable(NpgsqlDataReader sqlReader)
//	{
//		_sqlReader = sqlReader;

//		BoundedChannelOptions options = new BoundedChannelOptions(1000)
//		{
//			AllowSynchronousContinuations = true,
//			FullMode = BoundedChannelFullMode.Wait,
//			Capacity = 1000
//		};
//		_channel = Channel.CreateBounded<AsyncDataRow>(options);
//		_columns = Enumerable.Range(0, _sqlReader.FieldCount).Select(_sqlReader.GetName).ToList();
//	}

//	public async Task ReadAsync(CancellationToken cancelToken)
//	{
//		while (_sqlReader.Read())
//		{
//			if (cancelToken.IsCancellationRequested)
//				return;

//			var dict = Enumerable.Range(0, _sqlReader.FieldCount)
//				.ToDictionary(_sqlReader.GetName, _sqlReader.GetValue);

//			await _channel.Writer.WriteAsync(new AsyncDataRow(dict));
//		}
//	}

//	public ChannelReader<AsyncDataRow> GetRowsReader()
//	{
//		return _channel.Reader;
//	}
//}

