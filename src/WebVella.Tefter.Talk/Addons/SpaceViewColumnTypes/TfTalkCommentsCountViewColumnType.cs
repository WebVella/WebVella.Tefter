namespace WebVella.Tefter.Talk;

public class TfTalkCommentsCountViewColumnType : ITfSpaceViewColumnTypeAddon
{
	public const string ID = "60ab4197-be46-4ebd-a6de-02e8d25450d3";
	public const string NAME = "Talk Comments count";
	public const string DESCRIPTION = "displays related comments count";
	public const string FLUENT_ICON_NAME = "CommentMultiple";
	public Guid Id { get; init; } = new Guid(ID);
	public string Name { get; init; } = NAME;
	public string Description { get; init; } = DESCRIPTION;
	public string FluentIconName { get; init; } = FLUENT_ICON_NAME;
	public List<TfSpaceViewColumnAddonDataMapping> DataMapping { get; init; } = new();
	public Guid? DefaultComponentId { get; init; }= new Guid(TfTalkCommentsCountComponent.ID);
	public List<string> FilterAliases { get; init; } = new();
	public List<string> SortAliases { get; init; } = new();
	public List<Guid> SupportedComponents { get; set; } = new();

}

