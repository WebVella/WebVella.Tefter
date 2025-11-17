namespace WebVella.Tefter.UI.Tests.Components;
public class FilterCardComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		TfServiceMock.Setup(s => s.GetDatasetColumnOptions(It.IsAny<Guid>())).Returns(new List<TfDatasetColumn>());
		var dataset = new TfDataset();
		dataset.Id = Guid.NewGuid();
		// Act
		var cut = Context.Render<TucFilterCard>(parameters => parameters
			.Add(p => p.DatasetId, dataset.Id));

		// Assert
		cut.Find(".tf-card");

		Context.Dispose();
	}

}
