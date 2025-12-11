using WebVella.Tefter.Models;

namespace WebVella.Tefter.UI.Components;

public partial class TucSpacePageQuickViewDialog : TfBaseComponent, IDialogContentComponent<TucSpacePageQuickViewDialogContext>
{
	[Parameter] public TucSpacePageQuickViewDialogContext? Content { get; set; } = null;
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;

	private string _error = string.Empty;
	private TfSpace _space;
	private TfSpacePage _spacePage;


	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_init();
	}

	private void _init()
	{
		try
		{
			if (Content is null) throw new Exception("Content is null");
			if (Content.SpacePageId == Guid.Empty) throw new Exception("SpacePageId is required");
			if (String.IsNullOrWhiteSpace(Content.DataIdentity)) throw new Exception("DataIdentity is required");
			if (String.IsNullOrWhiteSpace(Content.RelDataIdentity)) throw new Exception("DataIdentity is required");
			if (Content.RelIdentityValues is null || Content.RelIdentityValues.Count == 0) throw new Exception("RelIdentityValues is required");
			_spacePage = TfService.GetSpacePage(Content.SpacePageId);
			if (_spacePage is null) throw new Exception(LOC("the related space page no longer exists"));
			_space = TfService.GetSpace(_spacePage.SpaceId);
		}
		catch (Exception ex)
		{
			_error = ex.Message;
		}
	}

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}


	private Dictionary<string, object> _getDynamicComponentParams()
	{
		var dict = new Dictionary<string, object>();
		if (_spacePage is not null)
		{
			dict["Context"] = new TfSpacePageAddonContext
			{
				ComponentOptionsJson = _spacePage.ComponentOptionsJson,
				Icon = _spacePage.FluentIconName,
				Mode = TfComponentMode.QuickView,
				SpacePage = _spacePage,
				Space = _space,
				CurrentUser = TfAuthLayout.GetState().User,
				RelDataIdentityQueryInfo = new()
				{
					DataIdentity = Content!.DataIdentity,
					RelDataIdentity = Content!.RelDataIdentity,
					RelIdentityValues = Content!.RelIdentityValues,
				}
			};
		}


		return dict;
	}
}

public record TucSpacePageQuickViewDialogContext
{
	public Guid SpacePageId { get; set; }
	public string DataIdentity { get; set; } = null!;
	public string RelDataIdentity { get; set; } = null!;
	public List<string> RelIdentityValues { get; set; } = new();
}