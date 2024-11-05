namespace WebVella.Tefter.Database;

public interface ITfTransactionRollbackNotifyService
{
    void OnTransactionCommit();
    void OnTransactionRollback();
}