namespace WebVella.Tefter.Models;

public interface ITfDynamicComponent<T>
{
	TfComponentMode DisplayMode { get; set; }
	T Context { get; set; }
}
