using Domain.BoundedContext.Properties;
using Domain.DomainShared;

namespace Test.Properties;

/// <summary>
/// Pruebas unitarias para Tower (Entidad del BC Properties).
/// Verifica: construcción válida, guard clauses, estado inicial heredado de Entity,
/// y método de negocio UpdateNumber.
/// </summary>
public class TowerTests
{
    // ─────────────────────────────────────────────
    // Happy Path
    // ─────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidNumber_ShouldCreateSuccessfully()
    {
        // Act
        var tower = new TowerEntity("Torre 1");

        // Assert
        tower.Number.Should().Be("Torre 1");
    }

    [Fact]
    public void Constructor_WithValidData_ShouldInitializeEntityState()
    {
        // Act
        var tower = new TowerEntity("Torre A");

        // Assert
        tower.Id.Should().NotBeEmpty("el Id debe generarse automáticamente.");
        tower.Status.Should().Be("Active", "el estado inicial es activo.");
        tower.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        tower.UpdateAt.Should().BeNull("no se ha actualizado aún.");
    }

    [Fact]
    public void Constructor_WithExactly20Chars_ShouldCreateSuccessfully()
    {
        // Arrange — en el límite exacto es válido
        var exactNumber = new string('T', 20);

        // Act
        var act = () => new TowerEntity(exactNumber);

        // Assert
        act.Should().NotThrow("20 caracteres es el límite permitido, no debe fallar.");
    }

    // ─────────────────────────────────────────────
    // Guard Clauses — Number obligatorio
    // ─────────────────────────────────────────────

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrWhiteSpaceNumber_ShouldThrowDomainException(string? invalidNumber)
    {
        // Act
        var act = () => new TowerEntity(invalidNumber!);

        // Assert
        act.Should().ThrowExactly<DomainException>()
           .WithMessage("*torre*obligatorio*",
           "el número de la torre es obligatorio.");
    }

    // ─────────────────────────────────────────────
    // Guard Clauses — Number longitud máxima
    // ─────────────────────────────────────────────

    [Fact]
    public void Constructor_WithNumberExceeding20Chars_ShouldThrowDomainException()
    {
        // Arrange
        var longNumber = new string('T', 21);

        // Act
        var act = () => new TowerEntity(longNumber);

        // Assert
        act.Should().ThrowExactly<DomainException>()
           .WithMessage("*20*",
           "el número no puede superar los 20 caracteres.");
    }

    // ─────────────────────────────────────────────
    // Constructor ORM
    // ─────────────────────────────────────────────

    [Fact]
    public void OrmConstructor_WhenInvoked_ShouldNotThrow()
    {
        // El constructor sin parámetros es requerido por EF Core
        var act = () => new TowerEntity();

        act.Should().NotThrow(
            "el constructor para ORM no debe ejecutar invariantes de negocio.");
    }

    // ─────────────────────────────────────────────
    // Método de negocio — UpdateNumber
    // ─────────────────────────────────────────────

    [Fact]
    public void UpdateNumber_WithValidNumber_ShouldUpdateSuccessfully()
    {
        // Arrange
        var tower = new TowerEntity("Torre 1");

        // Act
        tower.UpdateNumber("Torre 2");

        // Assert
        tower.Number.Should().Be("Torre 2");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateNumber_WithInvalidNumber_ShouldThrowDomainException(string? invalidNumber)
    {
        // Arrange
        var tower = new TowerEntity("Torre 1");

        // Act
        var act = () => tower.UpdateNumber(invalidNumber!);

        // Assert
        act.Should().ThrowExactly<DomainException>()
           .WithMessage("*torre*obligatorio*");
    }

    [Fact]
    public void UpdateNumber_WithNumberExceeding20Chars_ShouldThrowDomainException()
    {
        // Arrange
        var tower = new TowerEntity("Torre 1");
        var longNumber = new string('T', 21);

        // Act
        var act = () => tower.UpdateNumber(longNumber);

        // Assert
        act.Should().ThrowExactly<DomainException>()
           .WithMessage("*20*");
    }

    // ─────────────────────────────────────────────
    // Igualdad por Identidad (Entity)
    // ─────────────────────────────────────────────

    [Fact]
    public void TwoTowers_WithDifferentIds_ShouldNotBeEqual()
    {
        // Arrange
        var tower1 = new TowerEntity("Torre 1");
        var tower2 = new TowerEntity("Torre 1");

        // Assert — son entidades, la igualdad es por identidad (Id), no por atributos
        tower1.Id.Should().NotBe(tower2.Id,
            "dos instancias distintas de Tower deben tener Ids diferentes.");
    }
}
