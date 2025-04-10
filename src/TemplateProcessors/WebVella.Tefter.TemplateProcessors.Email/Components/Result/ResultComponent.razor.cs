﻿namespace WebVella.Tefter.TemplateProcessors.Email.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.Email.Components.Result.ResultComponent", "WebVella.Tefter.TemplateProcessors.Email")]
public partial class ResultComponent : TfBaseComponent, 
	ITfRegionComponent<TfTemplateProcessorResultScreenRegionContext>
{
	public Guid Id { get; init; } = new Guid("b0ca06fa-4e26-49c4-a043-c0ec7960ab02");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "Email Template Result";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(EmailTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorResultScreenRegionContext RegionContext { get; init; }

	private EmailTemplateResult _result = null;
	private bool _isLoading = true;
	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (RegionContext is null) throw new Exception("Context is not defined");
	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			if (RegionContext.Template is not null && RegionContext.SpaceData is not null)
			{
				ITfTemplateResult result = TfService.ProcessTemplate(
					templateId: RegionContext.Template.Id,
					spaceDataId: RegionContext.SpaceData.Id,
					tfRecordIds: RegionContext.SelectedRowIds,
					preview: RegionContext.Preview
				); ;
				if (result is EmailTemplateResult)
				{
					_result = (EmailTemplateResult)result;
				}
			}

			_isLoading = false;
			StateHasChanged();
		}
	}
}