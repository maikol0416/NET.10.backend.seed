using Domain.BoundedContext.DocumentManagement;
using Domain.DomainShared;

namespace Test.DocumentManagement;

/// <summary>
/// Pruebas unitarias para SignatureValueObject.
/// Verifica: construcción válida, guard clauses por campo y igualdad estructural (record).
/// </summary>
public class SignatureValueObjectTests
{
    // ─────────────────────────────────────────────
    // Happy Path
    // ─────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        const string name = "Juan Pérez";
        const string rol  = "Gerente General";

        // Act
        var signature = new SignatureValueObject(name, rol);

        // Assert
        signature.Name.Should().Be(name);
        signature.Rol.Should().Be(rol);
    }

    // ─────────────────────────────────────────────
    // Guard Clauses — campo Name
    // ─────────────────────────────────────────────

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_WithNullOrEmptyName_ShouldThrowDomainException(string? invalidName)
    {
        // Act
        var act = () => new SignatureValueObject(invalidName!, "Director");

        // Assert
        act.Should().ThrowExactly<DomainException>()
           .WithMessage("*nombre*",
           "el nombre del firmante es obligatorio.");
    }

    // ─────────────────────────────────────────────
    // Guard Clauses — campo Rol
    // ─────────────────────────────────────────────

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_WithNullOrEmptyRol_ShouldThrowDomainException(string? invalidRol)
    {
        // Act
        var act = () => new SignatureValueObject("María López", invalidRol!);

        // Assert
        act.Should().ThrowExactly<DomainException>()
           .WithMessage("*rol*",
           "el rol del firmante es obligatorio.");
    }

    // ─────────────────────────────────────────────
    // Igualdad Estructural (record)
    // ─────────────────────────────────────────────

    [Fact]
    public void TwoSignatures_WithSameData_ShouldBeEqual()
    {
        // Arrange
        var sig1 = new SignatureValueObject("Carlos Ruiz", "Auditor");
        var sig2 = new SignatureValueObject("Carlos Ruiz", "Auditor");

        // Assert
        sig1.Should().Be(sig2,
            "los Value Objects son iguales si todos sus campos tienen el mismo valor.");
    }

    [Fact]
    public void TwoSignatures_WithDifferentRol_ShouldNotBeEqual()
    {
        // Arrange
        var sig1 = new SignatureValueObject("Carlos Ruiz", "Auditor");
        var sig2 = new SignatureValueObject("Carlos Ruiz", "Director");

        // Assert
        sig1.Should().NotBe(sig2,
            "los Value Objects son distintos si el rol difiere.");
    }

    // ─────────────────────────────────────────────
    // Colección de firmantes
    // ─────────────────────────────────────────────

    [Fact]
    public void SignatureList_WithMultipleSigners_ShouldContainAllElements()
    {
        // Arrange & Act
        var signatures = new List<SignatureValueObject>
        {
            new("Ana Torres",   "Representante Legal"),
            new("Luis Gómez",   "Gerente Financiero"),
            new("Rosa Herrera", "Revisora Fiscal"),
        };

        // Assert
        signatures.Should().HaveCount(3)
                  .And.OnlyContain(s => !string.IsNullOrEmpty(s.Name) && !string.IsNullOrEmpty(s.Rol),
                  "cada firmante debe tener nombre y rol.");
    }
}
