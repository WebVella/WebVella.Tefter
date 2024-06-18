namespace WebVella.Tefter;

public interface ITfColumn
{
	public TfColumnData Data { get; }
	public TfColumnFilter Filter { get; }
	public TfColumnSort Sort { get; }
}

