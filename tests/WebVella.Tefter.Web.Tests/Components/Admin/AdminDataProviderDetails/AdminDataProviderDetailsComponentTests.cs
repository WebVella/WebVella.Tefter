using WebVella.Tefter.DataProviders.Csv;

namespace WebVella.Tefter.Web.Tests.Components;
public class AdminDataProviderDetailsComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();

		var dataProvider = new TucDataProvider(){
			ProviderType = new TucDataProviderTypeInfo(new CsvDataProvider())
		};
		
		new CsvDataProvider { };
		Dispatcher.Dispatch(new SetAppStateAction(
		component: null,
		state: new TfAppState { Hash = Guid.NewGuid(), AdminDataProvider = dataProvider }));
		// Act
		var cut = Context.RenderComponent<TfAdminDataProviderDetails>();

		// Assert
		cut.Find(".tf-card");

		Context.DisposeComponents();
	}
}
