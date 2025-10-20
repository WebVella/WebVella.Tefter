using System.Numerics;

namespace WebVella.Tefter.Utility;

public static class IntegerExtensions
{
	public static int? ToInt32Safely(this decimal? value)
	{
		if (value == null) return null;
        
		if (value > int.MaxValue) return int.MaxValue;
		if (value < int.MinValue) return int.MinValue;
		return (int)value;
	}
    
	public static int? ToInt32Safely(this long? value)
	{
		if (value == null) return null;
        
		if (value > int.MaxValue) return int.MaxValue;
		if (value < int.MinValue) return int.MinValue;
		return (int)value;
	}
    
	public static int? ToInt32Safely(this short? value)
	{
		if (value == null) return null;
		return (int)value;
	}
    
	public static int? ToInt32Safely(this int? value)
	{
		return value; // Already int, just return as is
	}
    
	// For float nullable
	public static int? ToInt32Safely(this float? value)
	{
		if (value == null) return null;
        
		if (float.IsNaN(value.Value) || float.IsInfinity(value.Value))
			return int.MaxValue;
            
		if (value > int.MaxValue) return int.MaxValue;
		if (value < int.MinValue) return int.MinValue;
        
		return (int)value;
	}
    
	// For double nullable
	public static int? ToInt32Safely(this double? value)
	{
		if (value == null) return null;
        
		if (double.IsNaN(value.Value) || double.IsInfinity(value.Value))
			return int.MaxValue;
            
		if (value > int.MaxValue) return int.MaxValue;
		if (value < int.MinValue) return int.MinValue;
        
		return (int)value;
	}

}