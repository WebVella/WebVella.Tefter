namespace WebVella.Tefter;

public record TfDbTypeSupportLevel
{
	public TfDatabaseColumnType? Number { get; set; } = null;
	public TfDatabaseColumnType? Text { get; set; } = null;
	public TfDatabaseColumnType? Boolean { get; set; } = null;
	public TfDatabaseColumnType? DateTime { get; set; } = null;
	public TfDatabaseColumnType? Guid { get; set; } = null;

	//Constructors
	public TfDbTypeSupportLevel(TfSpaceViewColumnDataMappingDefinition definition)
	{
		//number
		foreach (var dbType in NumberSupportPriority)
		{
			if (definition.SupportedDatabaseColumnTypes.Contains(dbType))
			{
				this.Number = dbType;
				break;
			}
		}

		//text
		foreach (var dbType in TextSupportPriority)
		{
			if (definition.SupportedDatabaseColumnTypes.Contains(dbType))
			{
				this.Text = dbType;
				break;
			}
		}

		//boolean
		foreach (var dbType in BooleanSupportPriority)
		{
			if (definition.SupportedDatabaseColumnTypes.Contains(dbType))
			{
				this.Boolean = dbType;
				break;
			}
		}

		//datetime
		foreach (var dbType in DateTimeSupportPriority)
		{
			if (definition.SupportedDatabaseColumnTypes.Contains(dbType))
			{
				this.DateTime = dbType;
				break;
			}
		}

		//guid
		foreach (var dbType in GuidSupportPriority)
		{
			if (definition.SupportedDatabaseColumnTypes.Contains(dbType))
			{
				this.Guid = dbType;
				break;
			}
		}
	}

	public bool Supports(TfDatabaseColumnType dbType)
	{
		//number
		if (NumberSupportPriority.Contains(dbType))
		{
			if(Number is null) return false;
			var currentIndex = NumberSupportPriority.IndexOf(Number.Value);
			var targetIndex = NumberSupportPriority.IndexOf(dbType);
			if(currentIndex <= targetIndex) return true;
		}
		//text
		else if (TextSupportPriority.Contains(dbType))
		{
			if(Text is null) return false;
			var currentIndex = TextSupportPriority.IndexOf(Text.Value);
			var targetIndex = TextSupportPriority.IndexOf(dbType);
			if(currentIndex <= targetIndex) return true;
		}
		//bool
		else if (BooleanSupportPriority.Contains(dbType))
		{
			if(Boolean is null) return false;
			var currentIndex = BooleanSupportPriority.IndexOf(Boolean.Value);
			var targetIndex = BooleanSupportPriority.IndexOf(dbType);
			if(currentIndex <= targetIndex) return true;
		}		
		//datatime
		else if (DateTimeSupportPriority.Contains(dbType))
		{
			if(DateTime is null) return false;
			var currentIndex = DateTimeSupportPriority.IndexOf(DateTime.Value);
			var targetIndex = DateTimeSupportPriority.IndexOf(dbType);
			if(currentIndex <= targetIndex) return true;
		}			
		//guid
		else if (GuidSupportPriority.Contains(dbType))
		{
			if(Guid is null) return false;
			var currentIndex = GuidSupportPriority.IndexOf(Guid.Value);
			var targetIndex = GuidSupportPriority.IndexOf(dbType);
			if(currentIndex <= targetIndex) return true;
		}			
		
		
		return false;
	}

	public List<TfDatabaseColumnType> NumberSupportPriority =>
	[
		TfDatabaseColumnType.Number,
		TfDatabaseColumnType.LongInteger,
		TfDatabaseColumnType.Integer,
		TfDatabaseColumnType.ShortInteger
	];

	public List<TfDatabaseColumnType> TextSupportPriority =>
	[
		TfDatabaseColumnType.Text,
		TfDatabaseColumnType.ShortText
	];

	public List<TfDatabaseColumnType> BooleanSupportPriority =>
	[
		TfDatabaseColumnType.Boolean
	];

	public List<TfDatabaseColumnType> DateTimeSupportPriority =>
	[
		TfDatabaseColumnType.DateTime, TfDatabaseColumnType.DateOnly
	];

	public List<TfDatabaseColumnType> GuidSupportPriority =>
	[
		TfDatabaseColumnType.DateTime, TfDatabaseColumnType.DateOnly
	];
}