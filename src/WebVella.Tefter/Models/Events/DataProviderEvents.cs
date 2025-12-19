namespace WebVella.Tefter.Models;
/// <summary>
/// Base payload for all bookmark-related events.
/// Use base class to subscribe to all derivatives
/// </summary>
public abstract record TfDataProviderEventPayload(TfDataProvider DataProvider) : ITfEventPayload;
public record TfDataProviderCreatedEventPayload(TfDataProvider DataProvider) : TfDataProviderEventPayload(DataProvider);
public record TfDataProviderUpdatedEventPayload(TfDataProvider DataProvider) : TfDataProviderEventPayload(DataProvider);
public record TfDataProviderDeletedEventPayload(TfDataProvider DataProvider) : TfDataProviderEventPayload(DataProvider);

