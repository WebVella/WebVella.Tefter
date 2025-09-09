namespace WebVella.Tefter.Utility;
public static class IEnumerableExtensions
{
	public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int size)
	{
		T[] bucket = null;
		var count = 0;

		foreach (var item in source)
		{
			if (bucket == null)
				bucket = new T[size];

			bucket[count++] = item;

			if (count != size)
				continue;

			yield return bucket.Select(x => x);

			bucket = null;
			count = 0;
		}

		if (bucket != null && count > 0)
			yield return bucket.Take(count);
	}

	/// <summary>
	/// Determines whether two <see cref="List{string}"/> contain the same elements,
	/// ignoring case and order.
	/// </summary>
	/// <param name="first">First list.</param>
	/// <param name="second">Second list.</param>
	/// <returns>True if both lists have exactly the same items (case‑insensitive); otherwise false.</returns>
	public static bool MatchesList(
		this IList<string> first,
		IList<string> second)
	{
		// Quick null / count check – avoids extra work.
		if (ReferenceEquals(first, second))
			return true;                      // same reference
		if (first == null || second == null)
			return false;
		if (first.Count != second.Count)
			return false;

		// Use a case‑insensitive comparer for the hash set.
		var comparer = StringComparer.OrdinalIgnoreCase;

		// Build a multiset for each list.
		var firstMultiset = new Dictionary<string, int>(comparer);
		foreach (var item in first)
			IncrementCount(firstMultiset, item);

		var secondMultiset = new Dictionary<string, int>(comparer);
		foreach (var item in second)
			IncrementCount(secondMultiset, item);

		// Two multisets are equal if they have the same keys with identical counts.
		return firstMultiset.Count == secondMultiset.Count &&
			   !firstMultiset.ExceptBy(secondMultiset.Keys, kvp => kvp.Key).Any() &&
			   firstMultiset.All(kvp => secondMultiset.TryGetValue(kvp.Key, out var count) && count == kvp.Value);
	}

	private static void IncrementCount(Dictionary<string, int> dict, string key)
	{
		if (dict.ContainsKey(key))
			dict[key]++;
		else
			dict[key] = 1;
	}

}
