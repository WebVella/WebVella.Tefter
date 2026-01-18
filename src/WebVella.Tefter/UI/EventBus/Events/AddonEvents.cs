namespace WebVella.Tefter.UI.EventsBus;
/// <summary>
/// Addon events can be fired from other addons or the core
/// And may include notifications that will be applied only if
/// the addon that is listening is installed
/// </summary>
public record TfAddonEventPayload(Guid AddonId, string EventName, string EventJson) : ITfEventPayload;
