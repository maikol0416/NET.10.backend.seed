using Domain.BoundedContext.Properties;
using Domain.DomainShared;

namespace Test.Properties;

/// <summary>
/// Pruebas unitarias para CommonAreaValueObject.
/// Verifica: construcción válida, guard clauses por campo y igualdad estructural (record).
/// </summary>
public class CommonAreaValueObjectTests
{
    // ─────────────────────────────────────────────
    // Happy Path
    // ─────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        const string name        = "Piscina";
        const string description = "Piscina olímpica con área de descanso";

        // Act
        var commonArea = new CommonAreaEntity(name, description);

        // Assert
        commonArea.Name.Should().Be(name);
        commonArea.Description.Should().Be(description);
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
        var act = () => new CommonAreaEntity(invalidName!, "Descripción válida");

        // Assert
        act.Should().ThrowExactly<DomainException>(
            "el nombre del área común es obligatorio.");
    }

    // ─────────────────────────────────────────────
    // Guard Clauses — campo Description
    // ─────────────────────────────────────────────

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_WithNullOrEmptyDescription_ShouldThrowDomainException(string? invalidDescription)
    {
        // Act
        var act = () => new CommonAreaEntity("Salón Social", invalidDescription!);

        // Assert
        act.Should().ThrowExactly<DomainException>(
            "la descripción del área común es obligatoria.");
    }

    // ─────────────────────────────────────────────
    // Igualdad de campos de negocio
    // ─────────────────────────────────────────────

    [Fact]
    public void TwoCommonAreas_WithSameBusinessData_ShouldHaveEqualNameAndDescription()
    {
        // Arrange
        var area1 = new CommonAreaEntity("Gimnasio", "Área de ejercicio equipada");
        var area2 = new CommonAreaEntity("Gimnasio", "Área de ejercicio equipada");

        // Assert — las entidades tienen Ids distintos, pero sus campos de negocio deben coincidir
        area1.Name.Should().Be(area2.Name,
            "dos áreas comunes con el mismo nombre deben ser equivalentes en negocio.");
        area1.Description.Should().Be(area2.Description,
            "dos áreas comunes con la misma descripción deben ser equivalentes en negocio.");
    }

    [Fact]
    public void TwoCommonAreas_WithDifferentName_ShouldNotHaveEqualName()
    {
        // Arrange
        var area1 = new CommonAreaEntity("Gimnasio", "Área de ejercicio");
        var area2 = new CommonAreaEntity("Salón Social", "Área de ejercicio");

        // Assert
        area1.Name.Should().NotBe(area2.Name,
            "los campos de negocio difieren cuando los nombres son distintos.");
    }


    // ─────────────────────────────────────────────
    // Inmutabilidad — lista de áreas comunes
    // ─────────────────────────────────────────────

    [Fact]
    public void CommonAreaList_WithMultipleAreas_ShouldContainAllElements()
    {
        // Arrange & Act
        var areas = new List<CommonAreaEntity>
        {
            new("Piscina",   "Piscina con temperatura controlada"),
            new("Gimnasio",  "Equipado con máquinas modernas"),
            new("BBQ",       "Zona de parrilla con mesas"),
        };

        // Assert
        areas.Should().HaveCount(3)
             .And.OnlyContain(a => !string.IsNullOrEmpty(a.Name),
             "cada área debe tener nombre.");
    }
}
