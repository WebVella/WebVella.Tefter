namespace WebVella.Tefter.UI.EventsBus;
/// <summary>
/// Base payload for all bookmark-related events.
/// Use base class to subscribe to all derivatives
/// </summary>
public abstract record TfScreenEventPayload : ITfEventPayload;
public record TfPageOutdatedAlertEventPayload : TfScreenEventPayload;


