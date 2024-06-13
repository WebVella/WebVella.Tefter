namespace WebVella.Tefter;

public interface ITfApplication
{
	public Guid Id { get; }
	public string Name { get; }

	public void CreateDatabaseStructure(DatabaseBuilder db);

	public void RemoveDatabaseStructure(DatabaseBuilder db);

}
