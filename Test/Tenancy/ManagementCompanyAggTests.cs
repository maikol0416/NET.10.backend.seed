using Domain.BoundedContext.Tenancy;
using Domain.DomainShared;

namespace Test.Tenancy;

/// <summary>
/// Pruebas unitarias para ManagementCompanyAgg (Aggregate Root del BC Tenancy).
/// Verifica: construcción válida, invariantes de dominio y estado inicial heredado de Entity.
/// </summary>
public class ManagementCompanyAggTests
{
    // ─────────────────────────────────────────────
    // Builders de objetos válidos
    // ─────────────────────────────────────────────

    private static ManagementCompanyAgg CreateValid(
        string name          = "Administradora Los Robles",
        string nit           = "900123456-7",
        string contactEmail  = "contacto@losrobles.com",
        string contactPhone  = "3001234567") =>
        new(name, nit, contactEmail, contactPhone);

    // ─────────────────────────────────────────────
    // Happy Path
    // ─────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidData_ShouldCreateSuccessfully()
    {
        // Act
        var agg = CreateValid();

        // Assert
        agg.Name.Should().Be("Administradora Los Robles");
        agg.Nit.Should().Be("900123456-7");
        agg.ContactEmail.Should().Be("contacto@losrobles.com");
        agg.ContactPhone.Should().Be("3001234567");
    }

    [Fact]
    public void Constructor_WithValidData_ShouldInitializeEntityState()
    {
        // Act
        var agg = CreateValid();

        // Assert
        agg.Id.Should().NotBeEmpty("el Id debe generarse automáticamente.");
        agg.Status.Should().Be("Active", "el estado inicial es activo.");
        agg.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        agg.UpdateAt.Should().BeNull("no se ha actualizado aún.");
    }

    // ─────────────────────────────────────────────
    // Guard Clause — Name obligatorio
    // ─────────────────────────────────────────────

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_WithNullOrEmptyName_ShouldThrowDomainException(string? invalidName)
    {
        // Act
        var act = () => CreateValid(name: invalidName!);

        // Assert
        act.Should().ThrowExactly<DomainException>()
           .WithMessage("*nombre*",
           "el nombre es obligatorio.");
    }

    // ─────────────────────────────────────────────
    // Guard Clause — Nit obligatorio
    // ─────────────────────────────────────────────

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_WithNullOrEmptyNit_ShouldThrowDomainException(string? invalidNit)
    {
        // Act
        var act = () => CreateValid(nit: invalidNit!);

        // Assert
        act.Should().ThrowExactly<DomainException>()
           .WithMessage("*Nit*",
           "el Nit es obligatorio.");
    }

    // ─────────────────────────────────────────────
    // Guard Clause — ContactEmail obligatorio
    // ─────────────────────────────────────────────

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_WithNullOrEmptyContactEmail_ShouldThrowDomainException(string? invalidEmail)
    {
        // Act
        var act = () => CreateValid(contactEmail: invalidEmail!);

        // Assert
        act.Should().ThrowExactly<DomainException>()
           .WithMessage("*ContactEmail*",
           "el ContactEmail es obligatorio.");
    }

    // ─────────────────────────────────────────────
    // Guard Clause — ContactPhone obligatorio
    // ─────────────────────────────────────────────

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_WithNullOrEmptyContactPhone_ShouldThrowDomainException(string? invalidPhone)
    {
        // Act
        var act = () => CreateValid(contactPhone: invalidPhone!);

        // Assert
        act.Should().ThrowExactly<DomainException>()
           .WithMessage("*ContactPhone*",
           "el ContactPhone es obligatorio.");
    }

    // ─────────────────────────────────────────────
    // Encapsulamiento — constructor ORM
    // ─────────────────────────────────────────────

    [Fact]
    public void OrmConstructor_WhenInvoked_ShouldNotThrow()
    {
        // El constructor sin parámetros es requerido por EF Core
        // para hidratar la entidad desde la BD sin pasar por las invariantes.
        var act = () => new ManagementCompanyAgg();

        act.Should().NotThrow(
            "el constructor para ORM no debe ejecutar invariantes de negocio.");
    }

    // ─────────────────────────────────────────────
    // Métodos de negocio — Update
    // ─────────────────────────────────────────────

    [Fact]
    public void Update_WithValidData_ShouldUpdateFieldsSuccessfully()
    {
        // Arrange
        var agg = CreateValid();

        // Act
        agg.Update("Administradora Nuevo Milenio", "900999999-9", "nuevo@milenio.com", "3009999999");

        // Assert
        agg.Name.Should().Be("Administradora Nuevo Milenio");
        agg.Nit.Should().Be("900999999-9");
        agg.ContactEmail.Should().Be("nuevo@milenio.com");
        agg.ContactPhone.Should().Be("3009999999");
    }

    [Fact]
    public void Update_WithInvalidData_ShouldThrowDomainException()
    {
        // Arrange
        var agg = CreateValid();

        // Act
        var act = () => agg.Update("", "900999999-9", "nuevo@milenio.com", "3009999999");

        // Assert
        act.Should().ThrowExactly<DomainException>();
    }
}
