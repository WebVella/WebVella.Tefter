namespace WebVella.Tefter.Web.Models;

public class TucBaseSpaceNodeComponent : ComponentBase, ITfSpaceNodeComponent
{
	public virtual Guid Id {get; set;} = Guid.NewGuid();

	public virtual string Name {get; set;} = "";

	public virtual string Description {get; set;} = "";

	public ComponentDisplayMode DisplayMode {get; set;} = ComponentDisplayMode.Display;

	public ITfSpaceNodeComponentContext Context  {get; set;} 

	public virtual object GetData(IServiceProvider serviceProvider)
	{
		throw new NotImplementedException();
	}
}