using Domain.BoundedContext.People;
using Domain.BoundedContext.People.Aggregates;
using Domain.DomainShared;
using FluentAssertions;

namespace Test.People;

/// <summary>
/// Pruebas unitarias para OwnerAgg (Aggregate Root del BC People).
/// Verifica: construcción válida, invariantes de dominio y estado inicial heredado de Entity.
/// </summary>
public class OwnerAggTests
{
    // ─────────────────────────────────────────────
    // Builders de objetos válidos
    // ─────────────────────────────────────────────

    private static OwnerAgg CreateValid(
        string name = "Juan",
        string lastName = "Perez",
        DocumentTypeEnum documentType = DocumentTypeEnum.CC,
        string documentNumber = "1234567890",
        string phoneNumber = "3001234567",
        string email = "juan.perez@example.com",
        int? idTermsAndCondition = 1,
        string responseTermsAndCondition = "Aceptado",
        string mediaId = "media-123") =>
        new(name, lastName, documentType, documentNumber, phoneNumber, email, idTermsAndCondition, responseTermsAndCondition, mediaId);

    // ─────────────────────────────────────────────
    // Happy Path
    // ─────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidData_ShouldCreateSuccessfully()
    {
        // Act
        var agg = CreateValid();

        // Assert
        agg.Name.Should().Be("Juan");
        agg.LastName.Should().Be("Perez");
        agg.DocumentType.Should().Be(DocumentTypeEnum.CC);
        agg.DocumentNumber.Should().Be("1234567890");
        agg.PhoneNumber.Should().Be("3001234567");
        agg.Email.Should().Be("juan.perez@example.com");
        agg.IdTermsAndCondition.Should().Be(1);
        agg.ResponseTermsAndCondition.Should().Be("Aceptado");
        agg.MediaId.Should().Be("media-123");
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
    // Guard Clauses — Invariantes
    // ─────────────────────────────────────────────

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrWhiteSpaceName_ShouldThrowDomainException(string? invalidName)
    {
        var act = () => CreateValid(name: invalidName!);
        act.Should().ThrowExactly<DomainException>().WithMessage("*nombre*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrWhiteSpaceLastName_ShouldThrowDomainException(string? invalidLastName)
    {
        var act = () => CreateValid(lastName: invalidLastName!);
        act.Should().ThrowExactly<DomainException>().WithMessage("*apellido*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrWhiteSpaceDocumentNumber_ShouldThrowDomainException(string? invalidDocNumber)
    {
        var act = () => CreateValid(documentNumber: invalidDocNumber!);
        act.Should().ThrowExactly<DomainException>().WithMessage("*documento*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrWhiteSpaceEmail_ShouldThrowDomainException(string? invalidEmail)
    {
        var act = () => CreateValid(email: invalidEmail!);
        act.Should().ThrowExactly<DomainException>().WithMessage("*correo*");
    }

    [Theory]
    [InlineData("correo-sin-arroba")]
    [InlineData("correo@sin-dominio")]
    [InlineData("@dominio.com")]
    public void Constructor_WithInvalidEmailFormat_ShouldThrowDomainException(string invalidEmail)
    {
        var act = () => CreateValid(email: invalidEmail);
        act.Should().ThrowExactly<DomainException>().WithMessage("*formato correcto*");
    }

    // ─────────────────────────────────────────────
    // Encapsulamiento — constructor ORM
    // ─────────────────────────────────────────────

    [Fact]
    public void OrmConstructor_WhenInvoked_ShouldNotThrow()
    {
        var act = () => new OwnerAgg();
        act.Should().NotThrow("el constructor para ORM no debe ejecutar invariantes de negocio.");
    }

    // ─────────────────────────────────────────────
    // Encapsulamiento — DomainEvents vacíos en construcción
    // ─────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidData_DomainEventsShouldBeEmpty()
    {
        var agg = CreateValid();
        agg.DomainEvents.Should().BeEmpty("la creación del agregado aún no publica eventos en el constructor por defecto.");
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
        agg.Update("Pedro", "Gomez", DocumentTypeEnum.CE, "0987654321", "3007654321", "pedro.gomez@example.com", null, "Rechazado", "media-456");

        // Assert
        agg.Name.Should().Be("Pedro");
        agg.LastName.Should().Be("Gomez");
        agg.DocumentType.Should().Be(DocumentTypeEnum.CE);
        agg.DocumentNumber.Should().Be("0987654321");
        agg.PhoneNumber.Should().Be("3007654321");
        agg.Email.Should().Be("pedro.gomez@example.com");
        agg.IdTermsAndCondition.Should().BeNull();
        agg.ResponseTermsAndCondition.Should().Be("Rechazado");
        agg.MediaId.Should().Be("media-456");
    }

    [Fact]
    public void Update_WithInvalidData_ShouldThrowDomainException()
    {
        // Arrange
        var agg = CreateValid(name: "Juan");

        // Act
        var act = () => agg.Update("", "Gomez", DocumentTypeEnum.CE, "0987654321", "3007654321", "pedro.gomez@example.com", null, "Rechazado", "media-456");

        // Assert
        act.Should().ThrowExactly<DomainException>().WithMessage("*nombre*");
    }
}
