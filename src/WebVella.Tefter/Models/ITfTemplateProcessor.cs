﻿namespace WebVella.Tefter.Models;

public interface ITfTemplateProcessor
{
	public Guid Id { get; }
	public string Name { get; }
	public string Description { get; }
	public string FluentIconName { get; }
	public TfTemplateResultType ResultType { get; }
	public Type SettingsComponentType { get; }
	public Type ResultPreviewComponentType { get; }
	public Type ResultComponentType { get; }
	public Type HelpComponentType { get; }

	public ITfTemplateResult GenerateTemplateResult(
		TfTemplate template,
		TfDataTable data,
		object processSettings,
		IServiceProvider serviceProvider);

	public List<ValidationError> ProcessTemplateResult(
		TfTemplate template,
		TfDataTable data,
		object processSettings,
		IServiceProvider serviceProvider);

	public List<ValidationError> ValidateSettings(
		string settingsJson,
		IServiceProvider serviceProvider );

	public List<ValidationError> OnCreate(
		TfManageTemplateModel template,
		IServiceProvider serviceProvider );

	public void OnCreated(
		TfTemplate template,
		IServiceProvider serviceProvider);

	public List<ValidationError> OnUpdate(
		TfManageTemplateModel template,
		IServiceProvider serviceProvider);

	public void OnUpdated(
		TfTemplate template,
		IServiceProvider serviceProvider);

	public List<ValidationError> OnDelete(
		TfTemplate template,
		IServiceProvider serviceProvider);

	public void OnDeleted(
		TfTemplate template,
		IServiceProvider serviceProvider);
}