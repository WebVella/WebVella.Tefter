using WebVella.Tefter.DataProviders.Csv.Addons;

namespace WebVella.Tefter.UI.Tests.Components;
public class AdminDataProviderDetailsComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();

			var dataProvider = new TucDataProvider()
			{
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
}
