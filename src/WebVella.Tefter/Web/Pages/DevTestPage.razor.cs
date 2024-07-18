using WebVella.Tefter;
using Npgsql;

namespace WebVella.Tefter.Web.Pages;
public partial class DevTestPage : TfBasePage
{

	//const string connectionString = "Server=localhost;Port=5432;User Id=dev;Password=dev;Database=maxcom_production_20240523;Pooling=true;MinPoolSize=1;MaxPoolSize=100;CommandTimeout=120;";

	//public List<AsyncDataRow> data = new List<AsyncDataRow>();

	//protected override async Task OnInitializedAsync()
	//{
	//	NpgsqlConnection connection = new NpgsqlConnection(connectionString);
	//	NpgsqlDataReader reader = null;

	//	connection.Open();
	//	NpgsqlCommand cmd = new NpgsqlCommand("select * from rec_transfer_item", connection);
	//	reader = await cmd.ExecuteReaderAsync();

	//	AsyncDataTable dt = new AsyncDataTable(reader);

	//	var rowsReader = dt.GetRowsReader();

	//	CancellationTokenSource ctsPrint = new CancellationTokenSource();
	//	CancellationTokenSource ctsRead = new CancellationTokenSource();

	//	dt.ReadAsync(ctsRead.Token);

	//	Task.Run(async () =>
	//	{
	//		int counter = 0;

	//		List<AsyncDataRow> buffer = new List<AsyncDataRow>();
	//		await foreach (var row in rowsReader.ReadAllAsync(ctsPrint.Token))
	//		{
	//			buffer.Add(row);

	//			if (counter % 1000 == 0)
	//			{
	//				data.AddRange(buffer);
	//				buffer.Clear();
	//				await Task.Delay(1);
	//				await InvokeAsync(StateHasChanged);
	//			}
	//		}
	//	});

	//	await base.OnInitializedAsync();
	//}


}