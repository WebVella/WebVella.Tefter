//namespace WebVella.Tefter.Identity;

//internal class TfTempStorageSessionStoreService
//{
//	private static AsyncLock _lock = new AsyncLock();
//	private static Dictionary<string, Guid> _storageDict;

//	public TfTempStorageSessionStoreService()
//	{
//		_storageDict = new Dictionary<string, Guid>();
//	}

//	public void Set(string key, Guid userId)
//	{
//		lock (_lock.Lock())
//		{
//			_storageDict[key] = userId;
//		}
//	}

//	public Guid? Get(string key)
//	{
//		return new Guid(key);
//		lock (_lock.Lock())
//		{
//			if (_storageDict.ContainsKey(key))
//			{
//				var userId = _storageDict[key];
//				_storageDict.Remove(key);
//				return userId;
//			}

//			return null;
//		}
//	}
//}
