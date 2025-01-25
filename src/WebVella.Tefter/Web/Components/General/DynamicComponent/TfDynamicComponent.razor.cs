﻿namespace WebVella.Tefter.Web.Components;
public partial class TfDynamicComponent
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public Type Scope { get; set; }
	[Parameter] public TfBaseDynamicComponentContext Context { get; set; }

	private Dictionary<string, object> _context
	{
		get
		{
			var dict = new Dictionary<string, object>();
			dict["Context"] = Context;
			return dict;
		}
	}

	private ReadOnlyCollection<TfDynamicComponentMeta> _componentsMeta
	{
		get
		{
			if(Context is null) return (new List<TfDynamicComponentMeta>()).AsReadOnly();

			return UC.GetDynamicComponentsMetaForContext(
				context:Context.GetType(),
				scope:Scope
			);
		}
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		StateHasChanged();
	}
}