namespace WebVella.Tefter.Database.Dbo;
internal class TfTransactionRollbackNotifyService : ITfTransactionRollbackNotifyService
{
    public void OnTransactionCommit()
    {
        TfDboManager.ClearFullCache();
    }
    public void OnTransactionRollback()
    {
        TfDboManager.ClearFullCache();
    }
}