using Domain.BoundedContext.People;
using Domain.BoundedContext.People.Aggregates;
using Domain.DomainShared;
using FluentAssertions;

namespace Test.People;

/// <summary>
/// Pruebas unitarias para GuestAgg (Aggregate Root del BC People).
/// Verifica: construcción válida, invariantes de dominio y sincronización de GuestPermissions.
/// </summary>
public class GuestAggTests
{
    private static GuestAgg CreateValid(
        string name = "Ana",
        string lastName = "Torres",
        DocumentTypeEnum documentType = DocumentTypeEnum.CC,
        string documentNumber = "1122334455",
        string phoneNumber = "3001112233",
        string email = "ana.torres@example.com",
        string termsAndCondition = "v1.0",
        string responseTermsAndCondition = "Aceptado",
        string mediaId = "media-789",
        List<GuestPermissionEntity>? guestPermissions = null) =>
        new(name, lastName, documentType, documentNumber, phoneNumber, email,
            termsAndCondition, responseTermsAndCondition, mediaId, guestPermissions ?? new List<GuestPermissionEntity>());

    // ─────────────────────────────────────────────
    // Happy Path
    // ─────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidData_ShouldCreateSuccessfully()
    {
        var agg = CreateValid();

        agg.Name.Should().Be("Ana");
        agg.LastName.Should().Be("Torres");
        agg.DocumentType.Should().Be(DocumentTypeEnum.CC);
        agg.DocumentNumber.Should().Be("1122334455");
        agg.PhoneNumber.Should().Be("3001112233");
        agg.Email.Should().Be("ana.torres@example.com");
        agg.TermsAndCondition.Should().Be("v1.0");
        agg.ResponseTermsAndCondition.Should().Be("Aceptado");
        agg.MediaId.Should().Be("media-789");
        agg.GuestPermissions.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithValidData_ShouldInitializeEntityState()
    {
        var agg = CreateValid();

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
    public void Constructor_WithNullOrWhiteSpacePhoneNumber_ShouldThrowDomainException(string? invalidPhoneNumber)
    {
        var act = () => CreateValid(phoneNumber: invalidPhoneNumber!);
        act.Should().ThrowExactly<DomainException>().WithMessage("*teléfono*");
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

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrWhiteSpaceTermsAndCondition_ShouldThrowDomainException(string? invalidTerms)
    {
        var act = () => CreateValid(termsAndCondition: invalidTerms!);
        act.Should().ThrowExactly<DomainException>().WithMessage("*términos y condiciones*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrWhiteSpaceResponseTermsAndCondition_ShouldThrowDomainException(string? invalidResponse)
    {
        var act = () => CreateValid(responseTermsAndCondition: invalidResponse!);
        act.Should().ThrowExactly<DomainException>().WithMessage("*respuesta de términos*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrWhiteSpaceMediaId_ShouldThrowDomainException(string? invalidMediaId)
    {
        var act = () => CreateValid(mediaId: invalidMediaId!);
        act.Should().ThrowExactly<DomainException>().WithMessage("*MediaId*");
    }

    // ─────────────────────────────────────────────
    // Encapsulamiento — constructor ORM
    // ─────────────────────────────────────────────

    [Fact]
    public void OrmConstructor_WhenInvoked_ShouldNotThrow()
    {
        var act = () => new GuestAgg();
        act.Should().NotThrow("el constructor para ORM no debe ejecutar invariantes de negocio.");
    }

    // ─────────────────────────────────────────────
    // Métodos de negocio — Update
    // ─────────────────────────────────────────────

    [Fact]
    public void Update_WithValidData_ShouldUpdateFieldsSuccessfully()
    {
        var agg = CreateValid();

        agg.Update("Luisa", "Ramirez", DocumentTypeEnum.CE, "9988776655", "3009998877",
            "luisa.ramirez@example.com", "v2.0", "Rechazado", "media-999");

        agg.Name.Should().Be("Luisa");
        agg.LastName.Should().Be("Ramirez");
        agg.DocumentType.Should().Be(DocumentTypeEnum.CE);
        agg.DocumentNumber.Should().Be("9988776655");
        agg.PhoneNumber.Should().Be("3009998877");
        agg.Email.Should().Be("luisa.ramirez@example.com");
        agg.TermsAndCondition.Should().Be("v2.0");
        agg.ResponseTermsAndCondition.Should().Be("Rechazado");
        agg.MediaId.Should().Be("media-999");
    }

    [Fact]
    public void Update_WithInvalidData_ShouldThrowDomainException()
    {
        var agg = CreateValid();

        var act = () => agg.Update("", "Ramirez", DocumentTypeEnum.CE, "9988776655", "3009998877",
            "luisa.ramirez@example.com", "v2.0", "Rechazado", "media-999");

        act.Should().ThrowExactly<DomainException>().WithMessage("*nombre*");
    }

    // ─────────────────────────────────────────────
    // Sincronización de colecciones — UpdateGuestPermissions
    // ─────────────────────────────────────────────

    [Fact]
    public void UpdateGuestPermissions_WithIncomingPermissions_ShouldReplaceCollection()
    {
        var agg = CreateValid();
        var incoming = new List<GuestPermissionEntity>
        {
            new(DateTime.UtcNow, DateTime.UtcNow.AddDays(1), Guid.NewGuid()),
            new(DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(3), Guid.NewGuid())
        };

        agg.UpdateGuestPermissions(incoming);

        agg.GuestPermissions.Should().HaveCount(2);
    }

    [Fact]
    public void UpdateGuestPermissions_WithNull_ShouldClearCollection()
    {
        var agg = CreateValid(guestPermissions: new List<GuestPermissionEntity>
        {
            new(DateTime.UtcNow, DateTime.UtcNow.AddDays(1), Guid.NewGuid())
        });

        agg.UpdateGuestPermissions(null!);

        agg.GuestPermissions.Should().BeEmpty();
    }
}
