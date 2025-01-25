﻿namespace WebVella.Tefter.Models;

public interface ITfDynamicComponent<T> where T : TfBaseDynamicComponentContext
{
	public Guid Id { get; init;}
	public int PositionRank { get; init;}
	public string Name { get; init;}
	public string Description { get; init;}
	public string FluentIconName { get; init; }
	T Context { get; init; }
}

public class TfDynamicComponentMeta
{
	public Guid Id { get; init;}
	public int PositionRank { get; init;}
	public string Name { get; init;}
	public string Description { get; init;}
	public string FluentIconName { get; init;}
	public Type ComponentType { get; init; }
	public List<string> ScopeTypeFullNames { get; init; } = new();
}