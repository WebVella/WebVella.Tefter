namespace WebVella.Tefter.Models;

public interface ITfComponentContext<T>
{
	TfComponentMode DisplayMode { get; set; }
	T Context { get; set; }
}
