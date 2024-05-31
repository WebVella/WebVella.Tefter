namespace WebVella.Tefter.Database.Internal;

internal class DbContext : IDisposable
{
    private static AsyncLocal<string> currentDbContextId = new AsyncLocal<string>();
    private static ConcurrentDictionary<string, DbContext> dbContextDict = new ConcurrentDictionary<string, DbContext>();

    public DbConfigurationService Configuration { get; private set; }
    private readonly object lockObj = new object();
    public NpgsqlTransaction Transaction { get { return transaction; } }
    internal Stack<DbConnection> connectionStack;
    private NpgsqlTransaction transaction;

    public static DbContext Current
    {
        get
        {
            if (currentDbContextId == null || string.IsNullOrWhiteSpace(currentDbContextId.Value))
                return null;

            DbContext context = null;
            dbContextDict.TryGetValue(currentDbContextId.Value, out context);
            return context;
        }
    }

    private DbContext(DbConfigurationService configuration)
    {
        Configuration = configuration;
        connectionStack = new Stack<DbConnection>();
    }

    public DbConnection CreateConnection()
    {
        DbConnection con = null;
        if (transaction != null)
            con = new DbConnection(transaction, this);
        else
            con = new DbConnection(Configuration.ConnectionString, this);

        connectionStack.Push(con);
        return con;
    }

    public bool CloseConnection(DbConnection conn)
    {
        lock (lockObj)
        {
            var dbConn = connectionStack.Peek();
            if (dbConn != conn)
                throw new Exception("You are trying to close connection, before closing inner connections.");

            connectionStack.Pop();
            return connectionStack.Count == 0;
        }
    }

    internal void EnterTransactionalState(NpgsqlTransaction transaction)
    {
        this.transaction = transaction;
    }

    internal void LeaveTransactionalState()
    {
        transaction = null;
    }

    internal static DbContext CreateContext(DbConfigurationService configuration)
    {
        currentDbContextId.Value = Guid.NewGuid().ToString();
        if (!dbContextDict.TryAdd(currentDbContextId.Value, new DbContext(configuration)))
            throw new Exception("Cannot create new context and store it into context dictionary");

        DbContext context;
        if (!dbContextDict.TryGetValue(currentDbContextId.Value, out context))
            throw new Exception("Cannot create new context and read it into context dictionary");

        return context;
    }

    internal static void CloseContext()
    {
        if (Current != null)
        {
            if (Current.transaction != null)
            {
                Current.transaction.Rollback();
                throw new Exception("Trying to release database context in transactional state. There is open transaction in created connections.");
            }
        }

        string idValue = null;
        if (currentDbContextId != null && !string.IsNullOrWhiteSpace(currentDbContextId.Value))
            idValue = currentDbContextId.Value;

        if (!string.IsNullOrWhiteSpace(idValue))
        {
            DbContext context;
            dbContextDict.TryRemove(idValue, out context);
            if (context != null)
                context.Dispose();

            currentDbContextId.Value = null;
        }
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
            CloseContext();
        }
    }
}
