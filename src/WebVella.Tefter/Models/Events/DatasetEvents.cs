namespace WebVella.Tefter.Models;
/// <summary>
/// Base payload for all bookmark-related events.
/// Use base class to subscribe to all derivatives
/// </summary>
public abstract record TfDatasetEventPayload(TfDataset Dataset) : ITfEventPayload;
public record TfDatasetCreatedEventPayload(TfDataset Dataset) : TfDatasetEventPayload(Dataset);
public record TfDatasetUpdatedEventPayload(TfDataset Dataset) : TfDatasetEventPayload(Dataset);
public record TfDatasetDeletedEventPayload(TfDataset Dataset) : TfDatasetEventPayload(Dataset);

