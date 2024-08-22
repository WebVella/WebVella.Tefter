﻿namespace WebVella.Tefter.Web.Pages;
public partial class FastAccessPage : TfBasePage
{
	[Parameter] public string Path { get; set; }
	[Inject] protected IState<ThemeState> ThemeState { get; set; }
	[Inject] protected IStateSelection<ScreenState, bool> ScreenStateSidebarExpanded { get; set; }
	protected override void OnInitialized()
	{
		base.OnInitialized();
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);
	}

}