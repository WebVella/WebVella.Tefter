namespace WebVella.Tefter.Web.Components;
public partial class TfValidationMessage : ComponentBase
{
	[Parameter] public string Field { get; set; }
	[Parameter] public long Index { get; set; } = -1;
	[Parameter] public List<ValidationError> Errors { get; set; }
	[Parameter] public string Class { get; set; }
	[Parameter] public string Style { get; set; }

	private string _cssClass => $"{Class} validation-message";
	private string _cssStyle => $"{Style}";
	private List<ValidationError> _errors
	{
		get => (Errors ?? new List<ValidationError>()).Where(x => x.PropertyName == Field && x.Index == Index).ToList();
	}

}