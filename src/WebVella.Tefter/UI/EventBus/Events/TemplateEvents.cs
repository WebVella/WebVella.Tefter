namespace WebVella.Tefter.UI.EventsBus;
/// <summary>
/// Base payload for all bookmark-related events.
/// Use base class to subscribe to all derivatives
/// </summary>
public abstract record TfTemplateEventPayload(TfTemplate Template) : ITfEventPayload;
public record TfTemplateCreatedEventPayload(TfTemplate Template) : TfTemplateEventPayload(Template);
public record TfTemplateUpdatedEventPayload(TfTemplate Template) : TfTemplateEventPayload(Template);
public record TfTemplateDeletedEventPayload(TfTemplate Template) : TfTemplateEventPayload(Template);

