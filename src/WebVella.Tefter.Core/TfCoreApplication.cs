namespace WebVella.Tefter.Core;

[TfApplicationType(
	id:"7d272612-0824-496a-a02d-2c144959ae73", 
	name:"Tefter Core Application",
	description:"This application include all core components.", 
	iconResourceName:"WebVella.Tefter.Core.Icon.png",
	allowMultipleInstances: false)]
public class TfCoreApplication : ITfApplication
{
	private Guid _id;
	private string _name;

	public Guid Id => _id;
	public String Name => _name;

	public void CreateDatabaseStructure(DatabaseBuilder db)
	{
		throw new NotImplementedException();
	}

	public void RemoveDatabaseStructure(DatabaseBuilder db)
	{
		throw new NotImplementedException();
	}
}
