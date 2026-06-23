using Domain.BoundedContext.Properties;
using Domain.DomainShared;

namespace Test.Properties;

/// <summary>
/// Pruebas unitarias para LocationValueObject.
/// Verifica: construcción válida, guard clauses por campo y igualdad estructural (record).
/// </summary>
public class LocationValueObjectTests
{
    // ─────────────────────────────────────────────
    // Datos válidos reutilizables
    // ─────────────────────────────────────────────
    private const string ValidNumber       = "123";
    private const string ValidDetail       = "Apto 4B";
    private const string ValidCountry      = "Colombia";
    private const string ValidCity         = "Medellín";
    private const string ValidNeighborhood = "El Poblado";

    private static LocationValueObject CreateValid() =>
        new(ValidNumber, ValidDetail, ValidCountry, ValidCity, ValidNeighborhood);

    // ─────────────────────────────────────────────
    // Happy Path
    // ─────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidData_ShouldCreateSuccessfully()
    {
        // Act
        var location = CreateValid();

        // Assert
        location.Number.Should().Be(ValidNumber);
        location.Detail.Should().Be(ValidDetail);
        location.Country.Should().Be(ValidCountry);
        location.City.Should().Be(ValidCity);
        location.Neighborhood.Should().Be(ValidNeighborhood);
    }

    // ─────────────────────────────────────────────
    // Guard Clauses — campo Number
    // ─────────────────────────────────────────────

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_WithNullOrEmptyNumber_ShouldThrowDomainException(string? invalidNumber)
    {
        // Act
        var act = () => new LocationValueObject(invalidNumber!, ValidDetail, ValidCountry, ValidCity, ValidNeighborhood);

        // Assert
        act.Should().ThrowExactly<DomainException>()
           .WithMessage("*Number*",
           "el número de la dirección es obligatorio.");
    }

    // ─────────────────────────────────────────────
    // Guard Clauses — campo Detail
    // ─────────────────────────────────────────────

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_WithNullOrEmptyDetail_ShouldThrowDomainException(string? invalidDetail)
    {
        // Act
        var act = () => new LocationValueObject(ValidNumber, invalidDetail!, ValidCountry, ValidCity, ValidNeighborhood);

        // Assert
        act.Should().ThrowExactly<DomainException>()
           .WithMessage("*Detail*",
           "el detalle de la dirección es obligatorio.");
    }

    // ─────────────────────────────────────────────
    // Guard Clauses — campo Country
    // ─────────────────────────────────────────────

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_WithNullOrEmptyCountry_ShouldThrowDomainException(string? invalidCountry)
    {
        // Act
        var act = () => new LocationValueObject(ValidNumber, ValidDetail, invalidCountry!, ValidCity, ValidNeighborhood);

        // Assert
        act.Should().ThrowExactly<DomainException>()
           .WithMessage("*Country*",
           "el país es obligatorio.");
    }

    // ─────────────────────────────────────────────
    // Guard Clauses — campo City
    // ─────────────────────────────────────────────

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_WithNullOrEmptyCity_ShouldThrowDomainException(string? invalidCity)
    {
        // Act
        var act = () => new LocationValueObject(ValidNumber, ValidDetail, ValidCountry, invalidCity!, ValidNeighborhood);

        // Assert
        act.Should().ThrowExactly<DomainException>()
           .WithMessage("*City*",
           "la ciudad es obligatoria.");
    }

    // ─────────────────────────────────────────────
    // Guard Clauses — campo Neighborhood
    // ─────────────────────────────────────────────

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_WithNullOrEmptyNeighborhood_ShouldThrowDomainException(string? invalidNeighborhood)
    {
        // Act
        var act = () => new LocationValueObject(ValidNumber, ValidDetail, ValidCountry, ValidCity, invalidNeighborhood!);

        // Assert
        act.Should().ThrowExactly<DomainException>()
           .WithMessage("*Neighborhood*",
           "el barrio/sector es obligatorio.");
    }

    // ─────────────────────────────────────────────
    // Igualdad Estructural (record)
    // ─────────────────────────────────────────────

    [Fact]
    public void TwoLocations_WithSameData_ShouldBeEqual()
    {
        // Arrange
        var location1 = CreateValid();
        var location2 = CreateValid();

        // Assert
        location1.Should().Be(location2,
            "los Value Objects son equivalentes en negocio si todos sus campos propios tienen el mismo valor.");
    }

    [Fact]
    public void TwoLocations_WithDifferentCity_ShouldNotBeEqual()
    {
        // Arrange
        var location1 = CreateValid();
        var location2 = new LocationValueObject(ValidNumber, ValidDetail, ValidCountry, "Bogotá", ValidNeighborhood);

        // Assert
        location1.Should().NotBe(location2,
            "los Value Objects son distintos si al menos un campo difiere.");
    }
}
