namespace WebVella.Tefter;

public interface  ITfRecipeAddon : ITfAddon
{
	public int SortIndex { get; init; }
	//creator
	public string Author { get; init; }
	//url for the documentation
	public string Website { get; init; }
	//ordered list of steps that will be performed
	public List<ITfRecipeStepAddon> Steps { get; init; }
}

