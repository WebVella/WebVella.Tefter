﻿using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.DocumentFile.Addons;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.DocumentFile.Addons.ScreenRegionComponents.Result.ResultComponent", "WebVella.Tefter.TemplateProcessors.DocumentFile")]
public partial class ResultComponent : TfBaseComponent, 
	ITfScreenRegionComponent<TfTemplateProcessorResultScreenRegionContext>
{
	public const string ID = "8eed6b14-101b-4fb9-863c-6e520b0196d8";
	public const string NAME = "Document Template Result";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "PuzzlePiece";
	public const int POSITION_RANK = 1000;
	public Guid Id { get; init; } = new Guid(ID);
	public string Name { get; init; } = NAME;
	public string Description { get; init; } = DESCRIPTION;
	public string FluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(DocumentFileTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorResultScreenRegionContext RegionContext { get; init; }

	private DocumentFileTemplateResult? _result = null;
	private bool _isLoading = true;
	private bool _showDetails = false;

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
				if (result is DocumentFileTemplateResult)
				{
					_result = (DocumentFileTemplateResult)result;
				}
			}

			_isLoading = false;
			StateHasChanged();
		}
	}


}