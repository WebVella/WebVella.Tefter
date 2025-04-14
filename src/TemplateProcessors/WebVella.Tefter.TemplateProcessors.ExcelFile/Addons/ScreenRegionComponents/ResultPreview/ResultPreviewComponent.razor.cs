using Microsoft.AspNetCore.Components.Forms;
using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.TemplateProcessors.ExcelFile.Addons;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.ExcelFile.Addons.ScreenRegionComponents.ResultPreview.ResultPreviewComponent", "WebVella.Tefter.TemplateProcessors.ExcelFile")]
public partial class ResultPreviewComponent : TfBaseComponent, 
	ITfRegionComponent<TfTemplateProcessorResultPreviewScreenRegionContext>
{
	public const string ID = "e6923a63-885f-4201-bab5-701867f7b952";
	public const string NAME = "Excel Template Result Preview";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "PuzzlePiece";
	public const int POSITION_RANK = 1000;
	public Guid Id { get; init; } = new Guid(ID);
	public string Name { get; init; } = NAME;
	public string Description { get; init; } = DESCRIPTION;
	public string FluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(ExcelFileTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorResultPreviewScreenRegionContext RegionContext { get; init; }

	private ExcelFileTemplatePreviewResult _preview = null;
	private bool _isLoading = true;
	private List<ValidationError> _previewValidationErrors = new();
	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (RegionContext is null) throw new Exception("Context is not defined");
		RegionContext.ValidatePreviewResult = _validatePreviewResult;
	}
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			if (RegionContext.Template is not null && RegionContext.SpaceData is not null)
			{
				ITfTemplatePreviewResult result = TfService.GenerateTemplatePreviewResult(
					templateId: RegionContext.Template.Id,
					spaceDataId: RegionContext.SpaceData.Id,
					tfRecordIds: RegionContext.SelectedRowIds
				);
				if (result is not ExcelFileTemplatePreviewResult)
				{
					throw new Exception("Preview result is not of type ExcelFileTemplatePreviewResult");
				}
				_preview = (ExcelFileTemplatePreviewResult)result;
				await RegionContext.PreviewResultChanged.InvokeAsync(_preview);
			}

			_isLoading = false;
			StateHasChanged();
		}
	}
	private List<ValidationError> _validatePreviewResult()
	{
		_previewValidationErrors = new List<ValidationError>();
		return _previewValidationErrors;
	}
}