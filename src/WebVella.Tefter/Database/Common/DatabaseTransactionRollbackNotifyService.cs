namespace WebVella.Tefter.Database;

public interface ITransactionRollbackNotifyService
{
    void OnTransactionCommit();
    void OnTransactionRollback();
}