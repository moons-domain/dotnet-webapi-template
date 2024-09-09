using FluentAssertions;
using Xunit.Abstractions;

namespace RichWebApi.Tests;

public class TypeEqualityComparerTests(ITestOutputHelper testOutputHelper) : UnitTest(testOutputHelper)
{
	[Fact]
	public void NotEqualIfDifferentRuntimeTypes() => new TypeEqualityComparer<IBaseInterface>()
		.Equals(new ClassA(), new ClassB())
		.Should()
		.BeFalse();

	[Fact]
	public void TrueIfSameRuntimeTypes() => new TypeEqualityComparer<IBaseInterface>()
		.Equals(new ClassA(), new ClassA())
		.Should()
		.BeTrue();

	[Fact]
	public void FalseIfFirstIsNull() => new TypeEqualityComparer<IBaseInterface>()
		.Equals(null, new ClassB())
		.Should()
		.BeFalse();

	[Fact]
	public void FalseIfSecondIsNull() => new TypeEqualityComparer<IBaseInterface>()
		.Equals(new ClassA(), null)
		.Should()
		.BeFalse();

	[Fact]
	public void TrueIfBothAreNull() => new TypeEqualityComparer<IBaseInterface>()
		.Equals(null, null)
		.Should()
		.BeTrue();

	[Fact]
	public void ObjectHashCodeIfNotNull()
	{
		var obj = new ClassA();
		var hashCode = obj.GetHashCode();
		new TypeEqualityComparer<IBaseInterface>()
			.GetHashCode(obj)
			.Should()
			.Be(hashCode);
	}

	[Fact]
	public void DefaultHashCodeIfObjectIsNull()
		=> new TypeEqualityComparer<IBaseInterface>()
			.GetHashCode(null!)
			.Should()
			.Be(0);

	private interface IBaseInterface
	{
	}

	private class ClassA : IBaseInterface
	{
	}

	private class ClassB : IBaseInterface
	{
	}
}