namespace WebVella.Tefter.UI.EventsBus;
/// <summary>
/// Base payload for all bookmark-related events.
/// Use base class to subscribe to all derivatives
/// </summary>
public abstract record TfRepositoryFileEventPayload(TfRepositoryFile RepositoryFile) : ITfEventPayload;
public record TfRepositoryFileCreatedEventPayload(TfRepositoryFile RepositoryFile) : TfRepositoryFileEventPayload(RepositoryFile);
public record TfRepositoryFileUpdatedEventPayload(TfRepositoryFile RepositoryFile) : TfRepositoryFileEventPayload(RepositoryFile);
public record TfRepositoryFileDeletedEventPayload(TfRepositoryFile RepositoryFile) : TfRepositoryFileEventPayload(RepositoryFile);

