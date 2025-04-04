namespace WebVella.Tefter.Web.Components;
public partial class TfValidationMessage : ComponentBase
{
	[Parameter] public string Field { get; set; }
	[Parameter] public long Index { get; set; } = -5;
	[Parameter] public List<ValidationError> Errors { get; set; }
	[Parameter] public string Class { get; set; }
	[Parameter] public string Style { get; set; }

	private string _cssClass => $"{Class} validation-message";
	private string _cssStyle => $"{Style}";
	private List<ValidationError> _errors
	{
		get
		{
			if(Field == "DefaultValue" && Index >= 0){ 
				var boz = 0;
			}
			var list = (Errors ?? new List<ValidationError>()).Where(x => x.PropertyName == Field && x.Index == Index).ToList();
			return list;
		}
	}

}