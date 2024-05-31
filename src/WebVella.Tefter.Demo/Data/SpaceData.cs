namespace WebVella.Tefter.Demo.Data;

public static class SpaceData
{
	public static List<Space> GetSpaces()
	{
		var result = new List<Space>();

		for (int i = 0; i < 10; i++)
		{
			var space = Space.GetFaker().Generate();
			for (int j = 0; j < 7; j++)
			{
				space.Items.Add(SpaceItem.GetFaker().Generate());
			}
			result.Add(space);
		}

		return result;
	}
}
