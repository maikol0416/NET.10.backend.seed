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

        var permission = new GuestPermissionEntity(startDate, endDate);

        permission.StartDate.Should().Be(startDate);
        permission.EndDate.Should().Be(endDate);
    }

    [Fact]
    public void Constructor_WithValidData_ShouldInitializeEntityState()
    {
        var permission = new GuestPermissionEntity(DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        permission.Id.Should().NotBeEmpty();
        permission.Status.Should().Be("Active");
        permission.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        permission.UpdateAt.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithStartDateEqualOrAfterEndDate_ShouldThrowDomainException()
    {
        var date = DateTime.UtcNow;

        var act = () => new GuestPermissionEntity(date, date);

        act.Should().ThrowExactly<DomainException>().WithMessage("*fecha de inicio*fecha de fin*");
    }

    [Fact]
    public void Constructor_WithStartDateAfterEndDate_ShouldThrowDomainException()
    {
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(-1);

        var act = () => new GuestPermissionEntity(startDate, endDate);

        act.Should().ThrowExactly<DomainException>().WithMessage("*fecha de inicio*fecha de fin*");
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateFieldsSuccessfully()
    {
        var permission = new GuestPermissionEntity(DateTime.UtcNow, DateTime.UtcNow.AddDays(1));
        var newStartDate = DateTime.UtcNow.AddDays(2);
        var newEndDate = newStartDate.AddDays(1);

        permission.Update(newStartDate, newEndDate);

        permission.StartDate.Should().Be(newStartDate);
        permission.EndDate.Should().Be(newEndDate);
    }

    [Fact]
    public void Update_WithInvalidDateRange_ShouldThrowDomainException()
    {
        var permission = new GuestPermissionEntity(DateTime.UtcNow, DateTime.UtcNow.AddDays(1));
        var date = DateTime.UtcNow;

        var act = () => permission.Update(date, date);

        act.Should().ThrowExactly<DomainException>().WithMessage("*fecha de inicio*fecha de fin*");
    }
}
