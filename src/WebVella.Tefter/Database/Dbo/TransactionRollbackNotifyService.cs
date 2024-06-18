namespace WebVella.Tefter.Database.Dbo;
internal class TransactionRollbackNotifyService : ITransactionRollbackNotifyService
{
    public void OnTransactionCommit()
    {
        DboManager.ClearFullCache();
    }
    public void OnTransactionRollback()
    {
        DboManager.ClearFullCache();
    }
}