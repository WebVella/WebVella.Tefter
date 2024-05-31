namespace WebVella.Tefter.Database.Internal;

internal interface ITransactionRollbackNotifyService
{
    void OnTransactionCommit();
    void OnTransactionRollback();
}

internal class TransactionRollbackNotifyService : ITransactionRollbackNotifyService
{
    public void OnTransactionCommit()
    {
    }

    public void OnTransactionRollback()
    {
    }
}
