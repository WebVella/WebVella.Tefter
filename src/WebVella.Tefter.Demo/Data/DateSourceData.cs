namespace WebVella.Tefter.Demo.Data;

public static partial class SampleData
{
	public static List<DataSource> GetDataSource()
	{
		var result = new List<DataSource>();

		for (int i = 0; i < 500; i++)
		{
			result.Add(DataSource.GetFaker().Generate());
		}

		return result;
	}
}
