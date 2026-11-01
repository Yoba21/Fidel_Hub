using NetArchTest.Rules;
using School.Shared.Primitives;
using Xunit;

namespace School.ArchitectureTests;

public class ArchitectureTests
{
    [Fact]
    public void SharedEntities_ShouldInheritFromEntity_OrImplementInterfaces()
    {
        var result = Types.InAssembly(typeof(Entity).Assembly)
            .That()
            .ResideInNamespace("School.Shared.Entities")
            .Should()
            .Inherit(typeof(Entity))
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void SharedProject_ShouldNotDependOn_ApiProject()
    {
        var result = Types.InAssembly(typeof(Entity).Assembly)
            .ShouldNot()
            .HaveDependencyOn("School.Api")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}
