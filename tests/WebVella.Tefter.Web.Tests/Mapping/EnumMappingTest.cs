using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WebVella.Tefter.Web.Tests.Mapping;
public class EnumMappingTest
{
	[Fact]
	public void EnumsAreMappedCorrectly()
	{
		//Given

		// Act
		var enumList = GetEnumTypesWithEnumMatchAttribute();

		foreach (var item in enumList)
		{
			var mappedType = GetEnumMatchAttributeValue(item);
			if (mappedType is null) continue;

			foreach (Enum itemVal in Enum.GetValues(item))
			{
				var hasMatch = false;
				foreach (Enum mapVal in Enum.GetValues(mappedType))
				{

					if (
						itemVal.ToString() == mapVal.ToString()
					&& Convert.ToInt32(itemVal) == Convert.ToInt32(mapVal)
					)
					{
						hasMatch = true;
					}
				}

				hasMatch.Should().BeTrue();
			}

			foreach (Enum mapVal in Enum.GetValues(mappedType))
			{
				var hasMatch = false;
				foreach (Enum itemVal in Enum.GetValues(item))
				{
					if (
						itemVal.ToString() == mapVal.ToString()
					&& Convert.ToInt32(itemVal) == Convert.ToInt32(mapVal)
					)
					{
						hasMatch = true;
					}
				}

				hasMatch.Should().BeTrue();
			}
		}
	}


	private static List<Type> GetEnumTypesWithEnumMatchAttribute()
	{
		var assemblies = AppDomain.CurrentDomain.GetAssemblies()
							.Where(a => !String.IsNullOrWhiteSpace(a.FullName) && !(a.FullName.ToLowerInvariant().StartsWith("microsoft.")
							   || a.FullName.ToLowerInvariant().StartsWith("system.")));
		List<Type> types = new List<Type>();
		foreach (var assembly in assemblies)
		{
			foreach (Type type in assembly.GetTypes())
			{
				if (type.GetCustomAttributes(typeof(TucEnumMatchAttribute), true).Length > 0
					&& type.IsEnum)
				{
					types.Add(type);
				}
			}
		}

		return types;
	}

	private static Type GetEnumMatchAttributeValue(Type type)
	{
		var attributes = type.GetCustomAttributes(typeof(TucEnumMatchAttribute), true);
		if (attributes.Length > 0)
		{
			return ((TucEnumMatchAttribute)attributes[0]).EnumType;
		}
		return null;
	}
}
