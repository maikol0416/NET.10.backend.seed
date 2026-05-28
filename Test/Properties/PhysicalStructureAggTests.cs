using Domain.BoundedContext.Properties;
using Domain.DomainShared;

namespace Test.Properties;

/// <summary>
/// Pruebas unitarias para PhysicalStructureAgg (Aggregate Root del BC Properties).
/// Verifica: construcción válida, invariantes de dominio, estado inicial heredado de Entity
/// y que las áreas comunes y ubicación se asignan correctamente.
/// </summary>
public class PhysicalStructureAggTests
{
    // ─────────────────────────────────────────────
    // Builders de objetos válidos
    // ─────────────────────────────────────────────

    private static LocationValueObject ValidLocation() =>
        new("Cra 45", "Apto 101", "Colombia", "Medellín", "Laureles");

    private static List<CommonAreaValueObject> ValidCommonAreas() =>
    [
        new("Piscina", "Piscina climatizada"),
        new("Gimnasio", "Zona de ejercicio"),
    ];

    private static PhysicalStructureAgg CreateValid(
        string name          = "Torres del Parque",
        string nit           = "900123456-7",
        int    unitCount     = 50,
        LocationValueObject? location    = null,
        List<CommonAreaValueObject>? areas = null) =>
        new(name, nit, unitCount, location ?? ValidLocation(), areas ?? ValidCommonAreas());

    // ─────────────────────────────────────────────
    // Happy Path
    // ─────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidData_ShouldCreateSuccessfully()
    {
        // Act
        var agg = CreateValid();

        // Assert
        agg.Name.Should().Be("Torres del Parque");
        agg.Nit.Should().Be("900123456-7");
        agg.UnitCount.Should().Be(50);
        agg.Location.Should().NotBeNull();
        agg.CommonsAreas.Should().HaveCount(2);
    }

    [Fact]
    public void Constructor_WithValidData_ShouldInitializeEntityState()
    {
        // Act
        var agg = CreateValid();

        // Assert
        agg.Id.Should().NotBeEmpty("el Id debe generarse automáticamente.");
        agg.Status.Should().Be("1", "el estado inicial es activo.");
        agg.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        agg.UpdateAt.Should().BeNull("no se ha actualizado aún.");
    }

    [Fact]
    public void Constructor_WithEmptyCommonAreas_ShouldCreateSuccessfully()
    {
        // Act — es válido tener una estructura sin áreas comunes
        var agg = CreateValid(areas: []);

        // Assert
        agg.CommonsAreas.Should().BeEmpty(
            "una estructura física puede existir sin áreas comunes registradas.");
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
           "el nombre de la estructura física es obligatorio.");
    }

    // ─────────────────────────────────────────────
    // Guard Clause — Name longitud máxima
    // ─────────────────────────────────────────────

    [Fact]
    public void Constructor_WithNameExceeding150Chars_ShouldThrowDomainException()
    {
        // Arrange
        var longName = new string('A', 151);

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
        // Arrange — en el límite exacto es válido
        var exactName = new string('A', 150);

        // Act
        var act = () => CreateValid(name: exactName);

        // Assert
        act.Should().NotThrow("150 caracteres es el límite permitido, no debe fallar.");
    }

    // ─────────────────────────────────────────────
    // Guard Clause — Location obligatoria
    // ─────────────────────────────────────────────

    [Fact]
    public void Constructor_WithNullLocation_ShouldThrowDomainException()
    {
        // Act
        var act = () => new PhysicalStructureAgg(
            "Torres del Norte", "900000001-1", 10, null!, ValidCommonAreas());

        // Assert
        act.Should().ThrowExactly<DomainException>()
           .WithMessage("*ubicación*",
           "la ubicación geográfica es obligatoria para una estructura física.");
    }

    // ─────────────────────────────────────────────
    // Encapsulamiento — constructor ORM
    // ─────────────────────────────────────────────

    [Fact]
    public void OrmConstructor_WhenInvoked_ShouldNotThrow()
    {
        // El constructor sin parámetros es requerido por EF Core
        // para hidratar la entidad desde la BD sin pasar por las invariantes.
        var act = () => new PhysicalStructureAgg();

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
        var agg = CreateValid();

        // Assert
        agg.DomainEvents.Should().BeEmpty(
            "la creación del agregado aún no publica eventos (aún no implementado).");
    }
}
