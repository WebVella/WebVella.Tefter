namespace WebVella.Tefter.Database.Internal;

internal interface ITransactionRollbackNotifyService
{
    void OnTransactionCommit();
    void OnTransactionRollback();
}