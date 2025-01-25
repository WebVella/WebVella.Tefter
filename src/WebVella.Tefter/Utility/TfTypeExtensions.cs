public static class TfTypeExtensions
{
	public static bool InheritsClass(this Type type, Type baseType)
	{
		if (type is null)
			throw new ArgumentException("The provided type is null.", nameof(type));
		if (baseType.IsInterface)
			throw new ArgumentException("The provided type must not be an interface.", nameof(baseType));
		Type currentType = type;
		while (currentType != null)
		{
			if (currentType == baseType)
				return true;

			currentType = currentType.BaseType;
		}
		return false;
	}
	public static bool InheritsGenericClass(this Type type, Type genericBaseType, Type genericArgumentType)
	{
		if (type is null)
			throw new ArgumentException("The provided type is null.", nameof(type));
		if (genericBaseType == null)
			throw new ArgumentNullException(nameof(genericBaseType));
		if (!genericBaseType.IsGenericType)
			throw new ArgumentException("The provided base type must be a generic type.", nameof(genericBaseType));

		// Traverse the type hierarchy
		Type currentType = type;
		while (currentType != null && currentType != typeof(object))
		{
			if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == genericBaseType)
			{
				// If a specific generic argument type is provided, check it
				Type[] typeArguments = currentType.GetGenericArguments();
				if (typeArguments.Length == 1 && typeArguments[0] == genericArgumentType)
				{
					return true;
				}
			}
			currentType = currentType.BaseType;
		}

		return false;
	}

	public static bool ImplementsInterface(this Type type, Type interfaceType)
	{
		if (type is null)
			throw new ArgumentException("The provided type is null.", nameof(type));
		if (!interfaceType.IsInterface)
			throw new ArgumentException("The provided type must be an interface.", nameof(interfaceType));

		return interfaceType.IsAssignableFrom(type);
	}

	public static bool ImplementsGenericInterface(this Type type, Type genericInterface)
	{
		if (type is null)
			throw new ArgumentException("The provided type is null.", nameof(type));
		if (!genericInterface.IsInterface)
			throw new ArgumentException("The provided type must be an interface.", nameof(genericInterface));

		return type.GetInterfaces()
			.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterface);
	}

	public static bool ImplementsGenericInterface(this Type type, Type genericInterface, Type genericArgument)
	{
		if (type is null)
			throw new ArgumentException("The provided type is null.", nameof(type));
		if (!genericInterface.IsInterface)
			throw new ArgumentException("The provided type must be an interface.", nameof(genericInterface));

		return type.GetInterfaces()
			.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterface)
			.Any(i => i.GetGenericArguments().Contains(genericArgument));
	}

	public static Type? GetGenericTypeFromGenericInterface(this Type type)
	{
		if (type is null)
			throw new ArgumentException("The provided type is null.", nameof(type));
		if (!type.IsInterface)
			throw new ArgumentException("The provided type must be a interface.", nameof(type));

		if (!type.IsGenericType)
			throw new ArgumentException("The provided type must be a generic interface.", nameof(type));

		return type.GetGenericArguments()?.FirstOrDefault();
	}

	public static List<Type> GetGenericTypeFromImplementedGenericInterface(this Type type, Type genericInterface)
	{
		if (type is null)
			throw new ArgumentException("The provided type is null.", nameof(type));

		var result = new List<Type>();
		
		if (genericInterface is null) return result;

		var implementedInterfaces = type.GetInterfaces()
			.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterface);

		if (!implementedInterfaces.Any()) return result;
		foreach (var implementedInterface in implementedInterfaces)
		{
			var implType = GetGenericTypeFromGenericInterface(implementedInterface);
			if (implType != null) result.Add(implType);
		}
		return result;
	}

	public static List<string> GetGenericTypeFromImplementedGenericInterfaceAsStringList(this Type type, Type genericInterface)
	{
		if (type is null)
			throw new ArgumentException("The provided type is null.", nameof(type));
		if (genericInterface is null) return new List<string>();
		return type.GetGenericTypeFromImplementedGenericInterface(genericInterface)
		.Where(x => x is not null && !String.IsNullOrWhiteSpace(x.FullName))
		.Select(x => x.FullName).ToList();
	}
}
