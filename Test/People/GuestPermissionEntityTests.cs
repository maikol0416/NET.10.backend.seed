using Domain.BoundedContext.People;
using Domain.DomainShared;
using FluentAssertions;

namespace Test.People;

public class GuestPermissionEntityTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateSuccessfully()
    {
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(1);
        var physicalStructureId = Guid.NewGuid();
        var apartmentId = Guid.NewGuid();

        var permission = new GuestPermissionEntity(startDate, endDate, physicalStructureId, apartmentId);

        permission.StartDate.Should().Be(startDate);
        permission.EndDate.Should().Be(endDate);
        permission.PhysicalStructureId.Should().Be(physicalStructureId);
        permission.ApartmentId.Should().Be(apartmentId);
    }

    [Fact]
    public void Constructor_WithValidData_ShouldInitializeEntityState()
    {
        var permission = new GuestPermissionEntity(DateTime.UtcNow, DateTime.UtcNow.AddDays(1), Guid.NewGuid());

        permission.Id.Should().NotBeEmpty();
        permission.Status.Should().Be("Active");
        permission.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        permission.UpdateAt.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithNullApartmentId_ShouldCreateSuccessfully()
    {
        // El apartamento es opcional: el permiso puede aplicar a toda la propiedad.
        var permission = new GuestPermissionEntity(DateTime.UtcNow, DateTime.UtcNow.AddDays(1), Guid.NewGuid());

        permission.ApartmentId.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithStartDateEqualOrAfterEndDate_ShouldThrowDomainException()
    {
        var date = DateTime.UtcNow;

        var act = () => new GuestPermissionEntity(date, date, Guid.NewGuid());

        act.Should().ThrowExactly<DomainException>().WithMessage("*fecha de inicio*fecha de fin*");
    }

    [Fact]
    public void Constructor_WithStartDateAfterEndDate_ShouldThrowDomainException()
    {
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(-1);

        var act = () => new GuestPermissionEntity(startDate, endDate, Guid.NewGuid());

        act.Should().ThrowExactly<DomainException>().WithMessage("*fecha de inicio*fecha de fin*");
    }

    [Fact]
    public void Constructor_WithEmptyPhysicalStructureId_ShouldThrowDomainException()
    {
        var act = () => new GuestPermissionEntity(DateTime.UtcNow, DateTime.UtcNow.AddDays(1), Guid.Empty);

        act.Should().ThrowExactly<DomainException>().WithMessage("*propiedad horizontal*");
    }

    [Fact]
    public void Constructor_WithEmptyApartmentId_ShouldThrowDomainException()
    {
        var act = () => new GuestPermissionEntity(DateTime.UtcNow, DateTime.UtcNow.AddDays(1), Guid.NewGuid(), Guid.Empty);

        act.Should().ThrowExactly<DomainException>().WithMessage("*apartamento*");
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateFieldsSuccessfully()
    {
        var permission = new GuestPermissionEntity(DateTime.UtcNow, DateTime.UtcNow.AddDays(1), Guid.NewGuid());
        var newStartDate = DateTime.UtcNow.AddDays(2);
        var newEndDate = newStartDate.AddDays(1);
        var newPhysicalStructureId = Guid.NewGuid();
        var newApartmentId = Guid.NewGuid();

        permission.Update(newStartDate, newEndDate, newPhysicalStructureId, newApartmentId);

        permission.StartDate.Should().Be(newStartDate);
        permission.EndDate.Should().Be(newEndDate);
        permission.PhysicalStructureId.Should().Be(newPhysicalStructureId);
        permission.ApartmentId.Should().Be(newApartmentId);
    }

    [Fact]
    public void Update_WithInvalidDateRange_ShouldThrowDomainException()
    {
        var permission = new GuestPermissionEntity(DateTime.UtcNow, DateTime.UtcNow.AddDays(1), Guid.NewGuid());
        var date = DateTime.UtcNow;

        var act = () => permission.Update(date, date, Guid.NewGuid(), null);

        act.Should().ThrowExactly<DomainException>().WithMessage("*fecha de inicio*fecha de fin*");
    }
}
