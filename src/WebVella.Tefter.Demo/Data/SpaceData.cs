namespace WebVella.Tefter.Demo.Data;

public static class SpaceData
{
	public static List<Space> GetSpaces()
	{
		var result = new List<Space>();
		//Ids need to be fixed for easier development
		var spaceIds = new List<Guid>(){ 
			new Guid("ce1b788f-b1b2-4d80-ba44-6ef21a3c1a05"),
			new Guid("db89b752-8885-447b-9ba9-af7b258d34dc"),
			new Guid("28dce3dd-a947-4c1b-9621-b4efdc496f45"),
		};
		var spaceItemIds = new List<Guid>(){
			new Guid("e94c9dcb-a90f-4074-a767-c9ac8f27d801"),
			new Guid("6a3e5c9b-8164-4d0c-9596-fb8ea856fe7a"),
			new Guid("9ecccc6f-6a3f-4f15-8f3e-dd24f325c0b9"),
			new Guid("88d62271-bdf2-42e9-8cb1-37b874cc43a5"),
			new Guid("14ae38d4-dd7e-440b-857e-f99de9fcab65"),
			new Guid("48189e7d-243f-4a05-a71a-db6a236ad70d"),
			new Guid("41d68149-45b8-46ce-b67c-23f17357586d"),
			new Guid("528ac40a-bfc5-494f-870a-4c7fd6b7d100"),
			new Guid("5f2f0607-b300-44e5-bf57-8d0d300e0cc4"),
			new Guid("3cb1f386-43e5-4966-a5d0-bff6d01c82ec"),
			new Guid("8c3b5196-ab9d-4203-8a61-b05688a25741"),
			new Guid("4fa5c29c-6d80-4713-8464-aba655752b54"),
			new Guid("b2808f5c-a52b-46e9-be3e-fe3a935f2119"),
			new Guid("b7819a1b-cb81-468d-bb76-335ad844d095"),
			new Guid("da270d5a-15f8-4386-907e-69a9e2ca66aa"),
			new Guid("58d94bff-2227-4d40-adef-a04dd09fcd87"),
			new Guid("63556014-c4b7-42ba-97c7-92a8be35ad1e"),
			new Guid("b7fd161b-d3f8-4438-a446-183e1cfe00b3"),
			new Guid("7100fb6d-9159-4a74-b0ee-774feeec7c69"),
			new Guid("6801d54a-0853-41d6-84ef-308061380d1b"),
			new Guid("20774230-9e23-49d5-a888-3978e5d9e7f0"),
		};
		for (int i = 0; i < 3; i++)
		{
			var space = Space.GetFaker().Generate();
			space.Id = spaceIds[i];
			for (int j = 0; j < 7; j++)
			{
				var spaceItem = SpaceItem.GetFaker().Generate();
				spaceItem.Id = spaceItemIds[(i*7) + j];
				spaceItem.Position = j+1;
				space.Items.Add(spaceItem);
			}
			result.Add(space);
		}

		return result;
	}
}
