namespace WebVella.Tefter.Extra.Addons;

public class IhSpaceRecipeAddon : ITfSpaceRecipeAddon
{
    public Guid AddonId { get; init; } = new Guid("00F6070C-E7FE-4FDA-A829-B51BFC21DCC4");
    public string AddonName { get; init; } = "IH001 Process Space";
    public string AddonDescription { get; init; } = "space based on specification for IH";
    public string AddonFluentIconName { get; init; } = "Building";
    public int SortIndex { get; init; } = 100;
    public string Author { get; init; } = "WebVella";
    public string Website { get; init; } = "https://tefter.webvella.com";
    public List<ITfRecipeStepAddon> Steps { get; init; } = new();

    public IhSpaceRecipeAddon()
    {
        Steps.Add(new TfCreateIHSpaceStep
        {
            Instance = new TfRecipeStepInstance
            {
                Visible = true,
                StepId = new Guid("ED49D41C-FE41-43CC-9F32-D7C0FA1036F8"),
                StepMenuTitle = "Get started",
                StepContentTitle = "IH001 Process Space",
            },
            Data = new TfCreateIHSpaceStepData()
            {
                BuildingCode = String.Empty
            }
        });        
    }
}