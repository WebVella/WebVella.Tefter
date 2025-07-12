namespace WebVella.Tefter.UI.Components;
public partial class TucRecipeStepForm : TfRecipeStepFormBase
{
	[Parameter] public ITfRecipeStepAddon Addon { get; set; }
	[Parameter] public bool IsSubstep { get; set; } = false;

	//private TfRecipeStepFormBase compRef;
	private string _renderedStepJson = null;

	protected override bool ShouldRender()
	{
		var stepJson = _serializeAddon();
		if (_renderedStepJson != stepJson)
		{
			_renderedStepJson = stepJson;
			return true;
		}
		return false;
	}
	public override void SubmitForm() { }

	private string _serializeAddon()
	{
		if (Addon is null) return null;

		var sb = new StringBuilder();
		sb.Append(Addon.AddonId);
		sb.Append(Addon.FormComponent?.GetType().FullName ?? "");
		sb.Append(Addon.Instance?.StepId.ToString() ?? "");
		sb.Append(Addon.Instance?.FormComponent?.GetType().FullName ?? "");
		sb.Append(JsonSerializer.Serialize(Addon.Instance?.Errors));
		return sb.ToString();
	}

	private Dictionary<string, object> _getDynamicComponentParams()
	{
		var dict = new Dictionary<string, object>();

		dict["Addon"] = Addon;

		return dict;
	}
}

