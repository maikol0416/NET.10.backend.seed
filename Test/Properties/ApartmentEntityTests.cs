using Domain.BoundedContext.Properties;
using Domain.DomainShared;
using FluentAssertions;
using Xunit;

namespace Test.Properties;

public class ApartmentEntityTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateSuccessfully()
    {
        var idOwner = Guid.NewGuid();
        var apartment = new ApartmentEntity("101", "50m2", idOwner);

        apartment.Number.Should().Be("101");
        apartment.Size.Should().Be("50m2");
        apartment.IdOwner.Should().Be(idOwner);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidNumber_ShouldThrowDomainException(string invalidNumber)
    {
        var idOwner = Guid.NewGuid();
        var action = () => new ApartmentEntity(invalidNumber, "50m2", idOwner);
        action.Should().ThrowExactly<DomainException>().WithMessage("*número*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidSize_ShouldThrowDomainException(string invalidSize)
    {
        var idOwner = Guid.NewGuid();
        var action = () => new ApartmentEntity("101", invalidSize, idOwner);
        action.Should().ThrowExactly<DomainException>().WithMessage("*tamaño*");
    }

    [Fact]
    public void Constructor_WithEmptyOwnerId_ShouldThrowDomainException()
    {
        var action = () => new ApartmentEntity("101", "50m2", Guid.Empty);
        action.Should().ThrowExactly<DomainException>().WithMessage("*propietario*");
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateFieldsSuccessfully()
    {
        var idOwner1 = Guid.NewGuid();
        var idOwner2 = Guid.NewGuid();
        var apartment = new ApartmentEntity("101", "50m2", idOwner1);

        apartment.Update("102", "60m2", idOwner2);

        apartment.Number.Should().Be("102");
        apartment.Size.Should().Be("60m2");
        apartment.IdOwner.Should().Be(idOwner2);
    }
}
