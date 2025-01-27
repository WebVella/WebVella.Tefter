using System.Collections.ObjectModel;
using WebVella.Tefter.Models;
using WebVella.Tefter.Web.Models;

namespace WebVella.Tefter.Tests;

public class UtilityTests
{
	protected static readonly AsyncLock locker = new AsyncLock();

	#region << TfTypeExtensions Class >>

	[Fact]
	public async Task Type_ShouldNotInheritClass()
	{
		using (await locker.LockAsync())
		{
			Type objType = typeof(UtilityTestsSimpleClass);
			bool? result = null;
			Func<bool> action = () => { result = objType.InheritsClass(typeof(UtilityTestsBaseClass)); return true; };
			action.Should().NotThrow();
			result.Should().BeFalse();
		}
	}

	[Fact]
	public async Task Type_ShouldInheritClass()
	{
		using (await locker.LockAsync())
		{
			Type objType = typeof(UtilityTestsChildClass);
			bool? result = null;
			Func<bool> action = () => { result = objType.InheritsClass(typeof(UtilityTestsBaseClass)); return true; };
			action.Should().NotThrow();
			result.Should().BeTrue();
		}
	}

	[Fact]
	public async Task Type_ShouldNotInheritGenericClass()
	{
		using (await locker.LockAsync())
		{
			Type objType = typeof(UtilityTestsChildGenericClass);
			bool? result = null;
			Func<bool> action = () => { result = objType.InheritsGenericClass(typeof(UtilityTestsBaseGenericClass<>), typeof(UtilityTestsBaseClass)); return true; };
			action.Should().NotThrow();
			result.Should().BeFalse();
		}
	}

	[Fact]
	public async Task Type_ShouldInheritGenericClass()
	{
		using (await locker.LockAsync())
		{
			Type objType = typeof(UtilityTestsChildGenericClass);
			bool? result = null;
			Func<bool> action = () => { result = objType.InheritsGenericClass(typeof(UtilityTestsBaseGenericClass<>), typeof(UtilityTestsSimpleClass)); return true; };
			action.Should().NotThrow();
			result.Should().BeTrue();
		}
	}
	#endregion

	#region << TfTypeExtensions Interface >>

	[Fact]
	public async Task Type_ShouldNotImplementInterface()
	{
		using (await locker.LockAsync())
		{
			Type objType = typeof(UtilityTestsSimpleClass);
			bool? result = null;
			Func<bool> action = () => { result = objType.ImplementsInterface(typeof(ITfDataProviderType)); return true; };
			action.Should().NotThrow();
			result.Should().BeFalse();
		}
	}

	[Fact]
	public async Task Type_ShouldImplementInterface()
	{
		using (await locker.LockAsync())
		{
			Type objType = typeof(UtilityTestsDataProviderTypeClass);
			bool? result = null;
			Func<bool> action = () => { result = objType.ImplementsInterface(typeof(ITfDataProviderType)); return true; };
			action.Should().NotThrow();
			result.Should().BeTrue();
		}
	}

	[Fact]
	public async Task Type_ShouldNotImplementGenericInterface()
	{
		using (await locker.LockAsync())
		{
			Type objType = typeof(UtilityTestsSimpleClass);
			bool? result = null;
			Func<bool> action = () => { result = objType.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(UtilityTestsSimpleClass)); return true; };
			action.Should().NotThrow();
			result.Should().BeFalse();
		}
	}

	[Fact]
	public async Task Type_ShouldImplementGenericInterface()
	{
		using (await locker.LockAsync())
		{
			Type objType = typeof(UtilityTestsClassDynamicComponentWithoutScope);
			bool? result = null;
			Func<bool> action = () => { result = objType.ImplementsGenericInterface(typeof(ITfRegionComponent<>),typeof(TfDataProviderManageSettingsComponentContext)); return true; };
			action.Should().NotThrow();
			result.Should().BeTrue();
		}
	}


	[Fact]
	public async Task Type_GetGenericTypeFromGenericInterfaceFalse()
	{
		using (await locker.LockAsync())
		{
			Type objType = typeof(UtilityTestsSimpleClass);
			Type result = null;
			Func<bool> action = () => { result = objType.GetGenericTypeFromGenericInterface(); return true; };
			action.Should().Throw<ArgumentException>("The provided type must be a interface.");
		}
	}

	[Fact]
	public async Task Type_GetGenericTypeFromGenericInterfaceFalse2()
	{
		using (await locker.LockAsync())
		{
			Type objType = typeof(ITfDataProviderType);
			Type result = null;
			Func<bool> action = () => { result = objType.GetGenericTypeFromGenericInterface(); return true; };
			action.Should().Throw<ArgumentException>("he provided type must be a generic interface.");
		}
	}

	[Fact]
	public async Task Type_GetGenericTypeFromGenericInterfaceFalse3()
	{
		using (await locker.LockAsync())
		{
			Type objType = typeof(ITfRegionComponent<TfDataProviderManageSettingsComponentContext>);
			Type result = null;
			Func<bool> action = () => { result = objType.GetGenericTypeFromGenericInterface(); return true; };
			action.Should().NotThrow();
			result.Should().Be(typeof(TfDataProviderManageSettingsComponentContext));
		}
	}



	[Fact]
	public async Task Type_GetGenericTypeFromImplementedGenericInterfaceFalse()
	{
		using (await locker.LockAsync())
		{
			Type objType = typeof(UtilityTestsSimpleClass);
			List<Type> result = new();
			Func<bool> action = () => { result = objType.GetGenericTypeFromImplementedGenericInterface(typeof(ITfRegionComponent<>)); return true; };
			action.Should().NotThrow();
			result.Should().NotBeNull();
			result.Count.Should().Be(0);
		}
	}

	[Fact]
	public async Task Type_GetGenericTypeFromImplementedGenericInterfaceTrue()
	{
		using (await locker.LockAsync())
		{
			Type objType = typeof(UtilityTestsClassDynamicComponentWithoutScope);
			List<Type> result = new();
			Func<bool> action = () => { result = objType.GetGenericTypeFromImplementedGenericInterface(typeof(ITfRegionComponent<>)); return true; };
			action.Should().NotThrow();
			result.Should().NotBeNull();
			result.Count.Should().Be(1);
			result[0].Should().Be(typeof(TfDataProviderManageSettingsComponentContext));
		}
	}

	#endregion
}

#region << Helper Classes >>
public class UtilityTestsSimpleClass
{

}

public class UtilityTestsBaseClass
{

}

public class UtilityTestsBaseGenericClass<T>
{

}

public class UtilityTestsChildClass : UtilityTestsBaseClass
{

}

public class UtilityTestsChildGenericClass : UtilityTestsBaseGenericClass<UtilityTestsSimpleClass>
{

}

public class UtilityTestsClassDynamicComponentWithoutScope : ITfRegionComponent<TfDataProviderManageSettingsComponentContext>
{
	public TfDataProviderManageSettingsComponentContext Context { get; init; }
	public Guid Id { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
	public int PositionRank { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
	public string Name { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
	public string Description { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
	public string FluentIconName { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
}

public class UtilityTestsClassDynamicComponentWithScope : ITfRegionComponent<TfDataProviderManageSettingsComponentContext>, ITfComponentScope<UtilityTestsDataProviderTypeClass>
{
	public TfDataProviderManageSettingsComponentContext Context { get; init; }
	public Guid Id { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
	public int PositionRank { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
	public string Name { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
	public string Description { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
	public string FluentIconName { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
}

public class UtilityTestsDataProviderTypeClass : ITfDataProviderType
{
	public Guid Id => throw new NotImplementedException();

	public string Name => throw new NotImplementedException();

	public string Description => throw new NotImplementedException();

	public string FluentIconName => throw new NotImplementedException();

	public Type SettingsComponentType => throw new NotImplementedException();

	public ReadOnlyCollection<TfDatabaseColumnType> GetDatabaseColumnTypesForSourceDataType(string dataType)
	{
		throw new NotImplementedException();
	}

	public TfDataProviderSourceSchemaInfo GetDataProviderSourceSchema(TfDataProvider provider)
	{
		throw new NotImplementedException();
	}

	public ReadOnlyCollection<TfDataProviderDataRow> GetRows(TfDataProvider provider)
	{
		throw new NotImplementedException();
	}

	public ReadOnlyCollection<string> GetSupportedSourceDataTypes()
	{
		throw new NotImplementedException();
	}

	public List<ValidationError> Validate(string settingsJson)
	{
		throw new NotImplementedException();
	}
}

#endregion
