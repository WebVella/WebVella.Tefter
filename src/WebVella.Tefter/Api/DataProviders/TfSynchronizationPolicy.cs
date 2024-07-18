namespace WebVella.Tefter;

public enum TfSynchronizationPolicyComparisonType
{
	ByRowOrder = 0,
	BySharedKey = 1
}

public enum TfSynchronizationPolicyDuplicateResolution
{
	UpdateFirstDeleteOthers = 0,
	UpdateAll = 1,
	UpdateFirstOnly = 2
}


/*
 synchronization policy
1. Comparison
	1.1. by row order
	1.2. by column key combination
		1.2.1. by specific list of columns - select from shared keys
		1.2.2. by all columns
2. Filter 
	2.1. include 
		2.1.1. all - default wildcard option
		2.1.2. column (as string) - equals pattern, not equals pattern
		2.1.3. column (as string) - contains pattern, not contains pattern
		2.1.4. column (as string) -  is empty, is not empty
	2.2. exclude 
		2.2.1. column (as string) - equals pattern, not equals pattern
		2.2.2. column (as string) - contains pattern, not contains pattern
		2.2.3. column (as string) - is empty, is not empty
	-- note more options if we use column database type values >,< etc
3. Synchronization -

	  Duplicates resolution if comparison is done by key
		- duplicate rows (with same column combination key) first will be updated, other deleted
		- duplicate rows (with same column combination key) all will be updated
		- duplicate rows (with same column combination key) only first will be updated, others will not be deleted or updated
				
*/

//TODO Rumen - implement filter at later stage

public record TfSynchronizationPolicy
{
	public TfSynchronizationPolicyComparisonType ComparisonType { get; set; } = TfSynchronizationPolicyComparisonType.ByRowOrder;
	public string SharedKeyName { get; set; } = null;
	public TfSynchronizationPolicyDuplicateResolution DuplicateResolution { get; set; } = TfSynchronizationPolicyDuplicateResolution.UpdateFirstDeleteOthers;
}
