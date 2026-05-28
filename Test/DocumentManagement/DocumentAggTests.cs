using Domain.BoundedContext.DocumentManagement;
using Domain.DomainShared;

namespace Test.DocumentManagement;

/// <summary>
/// Pruebas unitarias para DocumentAgg (Aggregate Root del BC DocumentManagement).
/// Verifica: construcción válida, invariantes de dominio, estado heredado de Entity
/// y el manejo de la colección de firmantes (Signatures).
/// </summary>
public class DocumentAggTests
{
    // ─────────────────────────────────────────────
    // Builders de objetos válidos
    // ─────────────────────────────────────────────

    private static List<SignatureValueObject> ValidSignatures() =>
    [
        new("Ana Torres",  "Representante Legal"),
        new("Luis Gómez",  "Gerente Financiero"),
    ];

    private static DocumentAgg CreateValid(
        string name              = "Contrato de Arrendamiento 2024",
        string? description      = "Documento contractual estándar",
        string path              = "/documentos/contratos/2024/arrendamiento.pdf",
        List<SignatureValueObject>? signatures = null) =>
        new(name, description, path, signatures ?? ValidSignatures());

    // ─────────────────────────────────────────────
    // Happy Path
    // ─────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidData_ShouldCreateSuccessfully()
    {
        // Act
        var doc = CreateValid();

        // Assert
        doc.Name.Should().Be("Contrato de Arrendamiento 2024");
        doc.Description.Should().Be("Documento contractual estándar");
        doc.Path.Should().Be("/documentos/contratos/2024/arrendamiento.pdf");
        doc.Signatures.Should().HaveCount(2);
    }

    [Fact]
    public void Constructor_WithNullDescription_ShouldCreateSuccessfully()
    {
        // La descripción es opcional (string?)
        var act = () => CreateValid(description: null);

        act.Should().NotThrow("la descripción del documento es opcional.");
    }

    [Fact]
    public void Constructor_WithValidData_ShouldInitializeEntityState()
    {
        // Act
        var doc = CreateValid();

        // Assert
        doc.Id.Should().NotBeEmpty("el Id debe generarse automáticamente.");
        doc.Status.Should().Be("1", "el estado inicial es activo.");
        doc.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        doc.UpdateAt.Should().BeNull("el documento no ha sido actualizado aún.");
    }

    [Fact]
    public void Constructor_WithEmptySignatures_ShouldCreateSuccessfully()
    {
        // Un documento puede existir aún sin firmantes asignados
        var act = () => CreateValid(signatures: []);

        act.Should().NotThrow("un documento puede crearse sin firmantes iniciales.");
    }

    // ─────────────────────────────────────────────
    // Guard Clause — Name obligatorio
    // ─────────────────────────────────────────────

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrWhiteSpaceName_ShouldThrowDomainException(string? invalidName)
    {
        // Act
        var act = () => CreateValid(name: invalidName!);

        // Assert
        act.Should().ThrowExactly<DomainException>()
           .WithMessage("*nombre*",
           "el nombre del documento es obligatorio.");
    }

    // ─────────────────────────────────────────────
    // Guard Clause — Name longitud máxima
    // ─────────────────────────────────────────────

    [Fact]
    public void Constructor_WithNameExceeding150Chars_ShouldThrowDomainException()
    {
        // Arrange
        var longName = new string('X', 151);

        // Act
        var act = () => CreateValid(name: longName);

        // Assert
        act.Should().ThrowExactly<DomainException>()
           .WithMessage("*150*",
           "el nombre no puede superar los 150 caracteres.");
    }

    [Fact]
    public void Constructor_WithNameExactly150Chars_ShouldCreateSuccessfully()
    {
        // En el límite exacto debe ser válido
        var exactName = new string('X', 150);

        var act = () => CreateValid(name: exactName);

        act.Should().NotThrow("150 caracteres es el límite permitido, no debe fallar.");
    }

    // ─────────────────────────────────────────────
    // Guard Clause — Path obligatorio
    // ─────────────────────────────────────────────

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrWhiteSpacePath_ShouldThrowDomainException(string? invalidPath)
    {
        // Act
        var act = () => CreateValid(path: invalidPath!);

        // Assert
        act.Should().ThrowExactly<DomainException>()
           .WithMessage("*ruta*",
           "la ruta del documento es obligatoria.");
    }

    // ─────────────────────────────────────────────
    // Encapsulamiento — constructor ORM
    // ─────────────────────────────────────────────

    [Fact]
    public void OrmConstructor_WhenInvoked_ShouldNotThrow()
    {
        // El constructor sin parámetros es requerido por EF Core
        var act = () => new DocumentAgg();

        act.Should().NotThrow(
            "el constructor para ORM no debe ejecutar invariantes de negocio.");
    }

    // ─────────────────────────────────────────────
    // Encapsulamiento — DomainEvents vacíos en construcción
    // ─────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidData_DomainEventsShouldBeEmpty()
    {
        // Act
        var doc = CreateValid();

        // Assert
        doc.DomainEvents.Should().BeEmpty(
            "la creación del documento aún no publica eventos (aún no implementado).");
    }

    // ─────────────────────────────────────────────
    // Signatures — integridad de la colección
    // ─────────────────────────────────────────────

    [Fact]
    public void Constructor_WithSignatures_ShouldPreserveAllSigners()
    {
        // Arrange
        var signatures = new List<SignatureValueObject>
        {
            new("Ana Torres",    "Representante Legal"),
            new("Luis Gómez",    "Gerente Financiero"),
            new("Rosa Herrera",  "Revisora Fiscal"),
        };

        // Act
        var doc = CreateValid(signatures: signatures);

        // Assert
        doc.Signatures.Should().HaveCount(3)
           .And.Contain(s => s.Name == "Rosa Herrera" && s.Rol == "Revisora Fiscal",
           "todos los firmantes deben estar presentes en el documento.");
    }
}
