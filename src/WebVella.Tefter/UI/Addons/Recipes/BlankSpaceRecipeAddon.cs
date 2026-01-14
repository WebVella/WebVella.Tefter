using WebVella.Tefter.UI.Addons;

namespace WebVella.Tefter.Extra.Addons;

public class BlankSpaceRecipeAddon : ITfSpaceRecipeAddon
{
    public Guid AddonId { get; init; } = new Guid("8DAEE8E7-9D89-4008-9D74-D1AE6E76F9AD");
    public string AddonName { get; init; } = "Create Blanks Space";
    public string AddonDescription { get; init; } = "start with an empty space";
    public string AddonFluentIconName { get; init; } = "AddCircle";
    public int SortIndex { get; init; } = 1;
    public string Author { get; init; } = "WebVella";
    public string Website { get; init; } = "https://tefter.webvella.com";
    public List<ITfRecipeStepAddon> Steps { get; init; } = new();

    public BlankSpaceRecipeAddon()
    {
	    var createSpaceStep = new TfCreateSpaceRecipeStep()
	    {
		    Instance = new TfRecipeStepInstance
		    {
			    Visible = true,
			    StepId = new Guid("97AB4D92-EF07-4F29-A11E-0F761BCCEC3D"),
			    StepMenuTitle = "Create space",
		    },
		    Data = new TfCreateSpaceRecipeStepData()
		    {
			    SpaceId = Guid.Empty,
			    Name = "",
			    IsPrivate = false,
			    Roles = new(),
			    Color = TfColor.Teal500,
			    FluentIconName = "Globe",
			    Position = 100
		    }		    
	    };
	    Steps.Add(createSpaceStep);
   
    }
}