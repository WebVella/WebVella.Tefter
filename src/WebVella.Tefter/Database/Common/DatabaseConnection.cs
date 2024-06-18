namespace WebVella.Tefter.Database;

public class DatabaseConnection : IDisposable
{
    private Stack<string> transactionStack = new Stack<string>();
    internal NpgsqlTransaction transaction;
    internal NpgsqlConnection connection;
    private bool initialTransactionHolder = false;

    private DatabaseContext CurrentContext;

    internal DatabaseConnection(NpgsqlTransaction transaction, DatabaseContext suppliedContext)
    {
        CurrentContext = suppliedContext;
        this.transaction = transaction;
        connection = transaction.Connection;
    }

    internal DatabaseConnection(string connectionString, DatabaseContext suppliedContext)
    {
        CurrentContext = suppliedContext;
        transaction = null;
        connection = new NpgsqlConnection(connectionString);
        connection.Open();
    }

    public NpgsqlCommand CreateCommand(string code, CommandType commandType = CommandType.Text, List<NpgsqlParameter> parameters = null)
    {
        NpgsqlCommand command = null;
        if (transaction != null)
            command = new NpgsqlCommand(code, connection, transaction);
        else
            command = new NpgsqlCommand(code, connection);

        command.CommandType = commandType;
        if (parameters != null)
            command.Parameters.AddRange(parameters.ToArray());

        return command;
    }

    public void AcquireAdvisoryLock(long key)
    {
        NpgsqlCommand command = CreateCommand("SELECT pg_advisory_lock(@key);");
        command.Parameters.Add(new NpgsqlParameter("@key", key));
        using (var reader = command.ExecuteReader())
        {
            try { reader.Read(); } finally { reader.Close(); }
        }
    }

    public void UnlockAdvisoryLock(long key)
    {
        NpgsqlCommand command = CreateCommand("SELECT pg_advisory_unlock(@key);");
        command.Parameters.Add(new NpgsqlParameter("@key", key));
        using (var reader = command.ExecuteReader())
        {
            try { reader.Read(); } finally { reader.Close(); }
        }
    }

    public void AcquireAdvisoryLock(string key)
    {
        AcquireAdvisoryLock(StringKeyToLong(key));
    }

    public void UnlockAdvisoryLock(string key)
    {
        UnlockAdvisoryLock(StringKeyToLong(key));
    }


    private static long StringKeyToLong(string key)
    {
        long hashCode = 0;
        if (!string.IsNullOrEmpty(key))
        {
            //Unicode Encode Covering all characterset
            byte[] byteContents = Encoding.Unicode.GetBytes(key);
            System.Security.Cryptography.SHA256 hash = System.Security.Cryptography.SHA256.Create();// obsolite - new System.Security.Cryptography.SHA256CryptoServiceProvider();
            byte[] hashText = hash.ComputeHash(byteContents);
            //32Byte hashText separate
            //hashCodeStart = 0~7  8Byte
            //hashCodeMedium = 8~23  8Byte
            //hashCodeEnd = 24~31  8Byte
            //and Fold
            long hashCodeStart = BitConverter.ToInt64(hashText, 0);
            long hashCodeMedium = BitConverter.ToInt64(hashText, 8);
            long hashCodeEnd = BitConverter.ToInt64(hashText, 24);
            hashCode = hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;
        }
        return hashCode;
    }

    public void BeginTransaction()
    {
        if (transaction == null)
        {
            initialTransactionHolder = true;
            transaction = connection.BeginTransaction();
            CurrentContext.EnterTransactionalState(transaction);
        }

        string savePointName = "tr_" + Guid.NewGuid().ToString().Replace("-", "");
        transaction.Save(savePointName);
        transactionStack.Push(savePointName);
    }

    public void CommitTransaction()
    {
        if (transaction == null)
            throw new Exception("Trying to commit non existent transaction.");

        var savepointName = transactionStack.Pop();

        if (transactionStack.Count() == 0)
        {
            CurrentContext.LeaveTransactionalState();
            if (!initialTransactionHolder)
            {
                transaction.Rollback();
                transaction = null;
                throw new Exception("Trying to commit transaction started from another connection. The transaction is rolled back.");
            }
            transaction.Commit();
            transaction = null;

        }
    }

    public void RollbackTransaction()
    {
        if (transaction == null)
            throw new Exception("Trying to rollback non existent transaction.");

        var savepointName = transactionStack.Pop();
        transaction.Rollback(savepointName);

        if (transactionStack.Count == 0)
        {
            transaction.Rollback();
            CurrentContext.LeaveTransactionalState();
            transaction = null;
            if (!initialTransactionHolder)
                throw new Exception("Trying to rollback transaction started from another connection.The transaction is rolled back, but this exception is thrown to notify.");
        }
    }

    public void Close()
    {
        if (transaction != null && initialTransactionHolder)
        {
            transaction.Rollback();
            throw new Exception("Trying to close connection with pending transaction. The transaction is rolled back.");
        }

        if (transactionStack.Count > 0)
            throw new Exception("Trying to close connection with pending transaction. The transaction is rolled back.");

        CurrentContext.CloseConnection(this);
        if (transaction == null)
            connection.Close();
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
            Close();
        }
    }

}
