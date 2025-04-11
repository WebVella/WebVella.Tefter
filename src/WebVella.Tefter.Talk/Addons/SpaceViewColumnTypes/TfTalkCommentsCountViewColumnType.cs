namespace WebVella.Tefter.Talk;

public class TfTalkCommentsCountViewColumnType : ITfSpaceViewColumnTypeAddon
{
	public Guid Id { get; init; }
	public string Name { get; init; }
	public string Description { get; init; }
	public string FluentIconName { get; init; }
	public List<TfSpaceViewColumnAddonDataMapping> DataMapping { get; init; }
	public Guid? DefaultComponentId { get; init; }
	public List<string> FilterAliases { get; init; }
	public List<string> SortAliases { get; init; }
	public List<Guid> SupportedComponents { get; set; } = new();


	public TfTalkCommentsCountViewColumnType()
	{

		Id = new Guid(TalkConstants.TALK_APP_SPACE_VIEW_COLUMN_TYPE_COMMENTS_COUNT);

		Name = "Talk Comments count";

		Description = "displays related comments count";

		FluentIconName = "CommentMultiple";

		DataMapping = new List<TfSpaceViewColumnAddonDataMapping>
		{
		};

		FilterAliases = new List<string>() { };

		SortAliases = new List<string> { };

		DefaultComponentId = new Guid(TalkConstants.TALK_APP_SPACE_VIEW_COLUMN_COMPONENT_COMMENTS_COUNT);

	}
}

