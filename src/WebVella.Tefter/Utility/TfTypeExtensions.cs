public static class TfTypeExtensions
{
	//public static bool Implements(this Type type, Type genericInterface) 
 //   {
 //       return type.GetInterfaces()
 //           .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterface);
 //   }

	public static bool Implements(this Type type, Type genericInterface, Type genericArgument) 
    {
        return type.GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterface)
            .Any(i => i.GetGenericArguments().Contains(genericArgument));
    }

}
