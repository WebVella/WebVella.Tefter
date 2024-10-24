
namespace WebVella.Tefter.Web.PageComponents;
public partial class TfSpaceViewPageComponent : ITfSpaceNodeComponent
{
	public Guid Id { get; set; } = Guid.NewGuid();

	public string Name { get; set; } = "SpaceView";

	public string Description { get; set; } = "";

	public ComponentDisplayMode DisplayMode { get; set; } = ComponentDisplayMode.Form;

	public object GetData(IServiceProvider serviceProvider)
	{
		throw new NotImplementedException();
	}
}