public static class TfTypeExtensions
{
	public static bool InheritsClass(this Type type, Type baseType)
	{
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
		if (!interfaceType.IsInterface)
			throw new ArgumentException("The provided type must be an interface.", nameof(interfaceType));

		return interfaceType.IsAssignableFrom(type);
	}

	public static bool ImplementsGenericInterface(this Type type, Type genericInterface, Type genericArgument)
	{
		if (!genericInterface.IsInterface)
			throw new ArgumentException("The provided type must be an interface.", nameof(genericInterface));

		return type.GetInterfaces()
			.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterface)
			.Any(i => i.GetGenericArguments().Contains(genericArgument));
	}

}
