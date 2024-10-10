using System.Text.Json.Serialization.Metadata;

[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class JsonIncludePrivatePropertyAttribute : System.Attribute { }

public static partial class JsonExtensions
{
	public static Action<JsonTypeInfo> AddPrivateProperties<TAttribute>() where TAttribute : System.Attribute => typeInfo =>
	{
		if (typeInfo.Kind != JsonTypeInfoKind.Object)
			return;
		foreach (var type in typeInfo.Type.BaseTypesAndSelf().TakeWhile(b => b != typeof(object)))
			AddPrivateProperties(typeInfo, type, p => Attribute.IsDefined(p, typeof(TAttribute)));
	};

	public static Action<JsonTypeInfo> AddPrivateProperties(Type declaredType) => typeInfo =>
		AddPrivateProperties(typeInfo, declaredType, p => true);

	public static Action<JsonTypeInfo> AddPrivateProperty(Type declaredType, string propertyName) => typeInfo =>
	{
		if (typeInfo.Kind != JsonTypeInfoKind.Object || !declaredType.IsAssignableFrom(typeInfo.Type))
			return;
		var propertyInfo = declaredType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
		if (propertyInfo == null)
			throw new ArgumentException(string.Format("Private roperty {0} not found in type {1}", propertyName, declaredType));
		if (typeInfo.Properties.Any(p => p.GetMemberInfo() == propertyInfo))
			return;
		AddProperty(typeInfo, propertyInfo);
	};

	static void AddPrivateProperties(JsonTypeInfo typeInfo, Type declaredType, Func<PropertyInfo, bool> filter)
	{
		if (typeInfo.Kind != JsonTypeInfoKind.Object || !declaredType.IsAssignableFrom(typeInfo.Type))
			return;
		var propertyInfos = declaredType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic);
		foreach (var propertyInfo in propertyInfos.Where(p => p.GetIndexParameters().Length == 0 && filter(p)))
			AddProperty(typeInfo, propertyInfo);
	}

	static void AddProperty(JsonTypeInfo typeInfo, PropertyInfo propertyInfo)
	{
		if (propertyInfo.GetIndexParameters().Length > 0)
			throw new ArgumentException("Indexed properties are not supported.");
		var ignore = propertyInfo.GetCustomAttribute<JsonIgnoreAttribute>();
		if (ignore?.Condition == JsonIgnoreCondition.Always)
			return;
		var name = propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
			?? typeInfo.Options?.PropertyNamingPolicy?.ConvertName(propertyInfo.Name)
			?? propertyInfo.Name;
		var property = typeInfo.CreateJsonPropertyInfo(propertyInfo.PropertyType, name);
		property.Get = CreateGetter(typeInfo.Type, propertyInfo.GetGetMethod(true));
		property.Set = CreateSetter(typeInfo.Type, propertyInfo.GetSetMethod(true));
		property.AttributeProvider = propertyInfo;
		property.CustomConverter = propertyInfo.GetCustomAttribute<JsonConverterAttribute>()?.ConverterType is { } converterType
			? (JsonConverter)Activator.CreateInstance(converterType)
			: null;
		typeInfo.Properties.Add(property);
	}

	delegate TValue RefFunc<TObject, TValue>(ref TObject arg);

	static Func<object, object> CreateGetter(Type type, MethodInfo method)
	{
		if (method == null)
			return null;
		var myMethod = typeof(JsonExtensions).GetMethod(nameof(JsonExtensions.CreateGetterGeneric), BindingFlags.NonPublic | BindingFlags.Static)!;
		return (Func<object, object>)(myMethod.MakeGenericMethod(new[] { type, method.ReturnType }).Invoke(null, new[] { method })!);
	}

	static Func<object, object> CreateGetterGeneric<TObject, TValue>(MethodInfo method)
	{
		if (method == null)
			throw new ArgumentNullException();
		if (typeof(TObject).IsValueType)
		{
			var func = (RefFunc<TObject, TValue>)Delegate.CreateDelegate(typeof(RefFunc<TObject, TValue>), null, method);
			return (o) => { var tObj = (TObject)o; return func(ref tObj); };
		}
		else
		{
			var func = (Func<TObject, TValue>)Delegate.CreateDelegate(typeof(Func<TObject, TValue>), method);
			return (o) => func((TObject)o);
		}
	}

	static Action<object, object> CreateSetter(Type type, MethodInfo method)
	{
		if (method == null)
			return null;
		var myMethod = typeof(JsonExtensions).GetMethod(nameof(JsonExtensions.CreateSetterGeneric), BindingFlags.NonPublic | BindingFlags.Static)!;
		return (Action<object, object>)(myMethod.MakeGenericMethod(new[] { type, method.GetParameters().Single().ParameterType }).Invoke(null, new[] { method })!);
	}

	static Action<object, object> CreateSetterGeneric<TObject, TValue>(MethodInfo method)
	{
		if (method == null)
			throw new ArgumentNullException();
		if (typeof(TObject).IsValueType)
		{
			return (o, v) => method.Invoke(o, new[] { v });
		}
		else
		{
			var func = (Action<TObject, TValue>)Delegate.CreateDelegate(typeof(Action<TObject, TValue>), method);
			return (o, v) => func((TObject)o, (TValue)v);
		}
	}

	static MemberInfo GetMemberInfo(this JsonPropertyInfo property) => (property.AttributeProvider as MemberInfo);

	static IEnumerable<Type> BaseTypesAndSelf(this Type type)
	{
		while (type != null)
		{
			yield return type;
			type = type.BaseType;
		}
	}
}
