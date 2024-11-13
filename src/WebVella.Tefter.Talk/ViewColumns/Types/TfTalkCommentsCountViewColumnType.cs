namespace WebVella.Tefter.Talk;

public class TfTalkCommentsCountViewColumnType : ITfSpaceViewColumnType
{
	public Guid Id { get; init; }
	public string Name { get; init; }
	public string Description { get; init; }
	public string Icon { get; init; }
	public List<TfSpaceViewColumnDataMapping> DataMapping { get; init; }
	public Type DefaultComponentType { get; init; }
	public Type CustomOptionsComponentType { get; init; }
	public List<Type> SupportedComponentTypes { get; init; }
	public List<string> FilterAliases { get; init; }
	public List<string> SortAliases { get; init; }
	public List<Guid> SupportedAddonTypes { get; init; } = new();


	public TfTalkCommentsCountViewColumnType()
	{

		Id = new Guid("60ab4197-be46-4ebd-a6de-02e8d25450d3");

		Name = "Talk Comments count";

		Description = "displays related comments count";

		Icon = "CommentMultiple";

		DataMapping = new List<TfSpaceViewColumnDataMapping>
		{
		};

		FilterAliases = new List<string>() { };

		SortAliases = new List<string> { };

		DefaultComponentType = typeof(TfTalkCommentsCountComponent);

		SupportedComponentTypes = new List<Type> {
			typeof(TfTalkCommentsCountComponent)
			};
	}
}

