using Domain.DomainShared;
using FluentAssertions;
using Xunit;

namespace Test.DomainShared;

/// <summary>
/// Pruebas unitarias para RolePermissionsPolicy.
/// Verifica que Administrator siempre resuelva a todos los módulos,
/// y que cualquier otro rol respete exactamente sus permisos asignados.
/// </summary>
public class RolePermissionsPolicyTests
{
    [Fact]
    public void IsAdministrator_WithExactMatch_ShouldReturnTrue()
    {
        RolePermissionsPolicy.IsAdministrator("Administrator").Should().BeTrue();
    }

    [Fact]
    public void IsAdministrator_IsCaseInsensitive()
    {
        RolePermissionsPolicy.IsAdministrator("administrator").Should().BeTrue();
        RolePermissionsPolicy.IsAdministrator("ADMINISTRATOR").Should().BeTrue();
    }

    [Fact]
    public void IsAdministrator_WithOtherRole_ShouldReturnFalse()
    {
        RolePermissionsPolicy.IsAdministrator("Supervisor").Should().BeFalse();
    }

    [Fact]
    public void Resolve_ForAdministrator_ShouldReturnAllModules_RegardlessOfAssignedPermissions()
    {
        var resolved = RolePermissionsPolicy.Resolve("Administrator", [ModuleEnum.Owner]);

        resolved.Should().BeEquivalentTo(Enum.GetValues<ModuleEnum>(),
            "Administrator tiene acceso a todos los módulos sin importar lo que tenga asignado explícitamente.");
    }

    [Fact]
    public void Resolve_ForAdministrator_WithNoAssignedPermissions_ShouldStillReturnAllModules()
    {
        var resolved = RolePermissionsPolicy.Resolve("Administrator", []);

        resolved.Should().BeEquivalentTo(Enum.GetValues<ModuleEnum>());
    }

    [Fact]
    public void Resolve_ForNonAdministratorRole_ShouldReturnExactlyAssignedPermissions()
    {
        var assigned = new[] { ModuleEnum.Owner, ModuleEnum.PhysicalStructure };

        var resolved = RolePermissionsPolicy.Resolve("Supervisor", assigned);

        resolved.Should().BeEquivalentTo(assigned);
    }

    [Fact]
    public void Resolve_ForNonAdministratorRole_WithNoPermissions_ShouldReturnEmpty()
    {
        var resolved = RolePermissionsPolicy.Resolve("Supervisor", []);

        resolved.Should().BeEmpty();
    }
}
