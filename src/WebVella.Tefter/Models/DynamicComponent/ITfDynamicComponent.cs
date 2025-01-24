namespace WebVella.Tefter.Models;

public interface ITfDynamicComponent<T> where T : TfBaseComponentContext
{
	Guid Id { get; set; }
	TfComponentMode DisplayMode { get; set; }
	T Context { get; set; }
}
