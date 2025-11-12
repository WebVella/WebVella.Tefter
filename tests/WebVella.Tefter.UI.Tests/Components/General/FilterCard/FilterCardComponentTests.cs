namespace WebVella.Tefter.UI.Tests.Components;
public class FilterCardComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		TfServiceMock.Setup(s => s.GetDatasetColumnOptions(It.IsAny<Guid>())).Returns(new List<TfDatasetColumn>());
		// Act
		var cut = Context.Render<TucFilterCard>(parameters => parameters
			.Add(p => p.DatasetId, (new TfDataset()).Id));

		// Assert
		cut.Find(".tf-card");

		Context.Dispose();
	}

}
