namespace WebVella.Tefter.Database.Internal;

internal class DbTransactionScope : IDisposable
{
    private ITransactionRollbackNotifyService _tranRNS = null;
    private bool _isCompleted = false;
    private bool _shouldDisposeObjects = true;
    private DbContext _dbCtx;
    private DbConnection _dbCon;
    private string _lockKey = null;

    public DbContext Context { get { return _dbCtx; } }
    public DbConnection Connection { get { return _dbCon; } }

    public string LockKey { get { return _lockKey; } }

    internal DbTransactionScope(DbConfigurationService configuration, ITransactionRollbackNotifyService tranRNS, string lockKey = null)
    {
        _lockKey = lockKey;
        _tranRNS = tranRNS;
        if (DbContext.Current != null)
        {
            _dbCtx = DbContext.Current;
            if (_dbCtx.connectionStack.Count > 0)
                _dbCon = _dbCtx.connectionStack.Peek();
            else
                _dbCon = _dbCtx.CreateConnection();

            _shouldDisposeObjects = false;
        }
        else
        {
            _dbCtx = DbContext.CreateContext(configuration);
            _dbCon = _dbCtx.CreateConnection();
        }
        _dbCon.BeginTransaction();

        if (!string.IsNullOrWhiteSpace(_lockKey))
            _dbCon.AcquireAdvisoryLock(_lockKey);
    }

    public void Complete()
    {
        if (_isCompleted)
            throw new Exception("DbTransactionScope is already completed.");

        _dbCon.CommitTransaction();

        if (!string.IsNullOrWhiteSpace(_lockKey))
            _dbCon.UnlockAdvisoryLock(_lockKey);

        _isCompleted = true;
        if (_tranRNS != null)
            _tranRNS.OnTransactionCommit();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (!_isCompleted)
            {
                if (_tranRNS != null)
                    _tranRNS.OnTransactionRollback();

                _dbCon.RollbackTransaction();

                if (!string.IsNullOrWhiteSpace(_lockKey))
                    _dbCon.UnlockAdvisoryLock(_lockKey);
            }

            if (_shouldDisposeObjects)
            {
                _dbCon.Dispose();
                _dbCon = null;

                _dbCtx.Dispose();
                _dbCtx = null;
            }
        }
    }
}
