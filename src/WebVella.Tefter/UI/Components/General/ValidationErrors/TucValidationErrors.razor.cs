namespace WebVella.Tefter.UI.Components;
public partial class TucValidationErrors : TfBaseComponent
{
	[Parameter] public List<ValidationError> Errors { get; set; } = new();
	[Parameter] public string Message { get; set; } = null;
	[Parameter] public string Class { get; set; } = null;
	[Parameter] public string Style { get; set; } = null;
}