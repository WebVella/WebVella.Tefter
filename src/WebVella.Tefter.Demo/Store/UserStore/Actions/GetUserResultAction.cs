namespace WebVella.Tefter.Demo.Store;

public partial class GetUserResultAction {
	public bool IsLoading { get; set; }

	public GetUserResultAction(){}
	public GetUserResultAction(bool isLoading){
		IsLoading = isLoading;
	}
}
