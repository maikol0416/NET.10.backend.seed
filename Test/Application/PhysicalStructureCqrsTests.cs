using Application.Base;
using Application.Dto;
using Domain.BoundedContext.Properties;
using FluentAssertions;
using Moq;

namespace Test.Application.PhysicalStructure;

public class PhysicalStructureCqrsTests
{
    private readonly Mock<IApplicationService<PhysicalStructureAgg, PhysicalStructureDto>> _serviceMock;
    private readonly Mock<IApplicationReadOnlyService<PhysicalStructureAgg, PhysicalStructureDto>> _readOnlyServiceMock;

    public PhysicalStructureCqrsTests()
    {
        _serviceMock = new Mock<IApplicationService<PhysicalStructureAgg, PhysicalStructureDto>>();
        _readOnlyServiceMock = new Mock<IApplicationReadOnlyService<PhysicalStructureAgg, PhysicalStructureDto>>();
    }

    [Fact]
    public async Task CreateHandler_ShouldCallCreateAsync_AndReturnDto()
    {
        // Arrange
        var handler = new CreateHandler<PhysicalStructureAgg, PhysicalStructureDto>(_serviceMock.Object);
        var dto = new PhysicalStructureDto { Name = "Estructura Nueva", Nit = "123456789-0" };
        var command = new CreateCommand<PhysicalStructureAgg, PhysicalStructureDto>(dto);

        _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(dto);
        _serviceMock.Verify(s => s.CreateAsync(dto), Times.Once);
    }

    [Fact]
    public async Task UpdateHandler_ShouldCallUpdateAsync_AndReturnDto()
    {
        // Arrange
        var handler = new UpdateHandler<PhysicalStructureAgg, PhysicalStructureDto>(_serviceMock.Object);
        var dto = new PhysicalStructureDto { Id = Guid.NewGuid(), Name = "Estructura Actualizada" };
        var command = new UpdateCommand<PhysicalStructureAgg, PhysicalStructureDto>(dto);

        _serviceMock.Setup(s => s.UpdateAsync(dto)).ReturnsAsync(dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(dto);
        _serviceMock.Verify(s => s.UpdateAsync(dto), Times.Once);
    }

    [Fact]
    public async Task DeleteHandler_ShouldCallDeleteEntity_AndReturnTrue()
    {
        // Arrange
        var handler = new DeleteHandler<PhysicalStructureAgg, PhysicalStructureDto>(_serviceMock.Object);
        var id = Guid.NewGuid();
        var command = new DeleteCommand<PhysicalStructureAgg, PhysicalStructureDto>(id);

        _serviceMock.Setup(s => s.DeleteEntity(id)).ReturnsAsync(true);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _serviceMock.Verify(s => s.DeleteEntity(id), Times.Once);
    }

    [Fact]
    public async Task GetByIdHandler_ShouldCallGetByIdAsync_AndReturnDto()
    {
        // Arrange
        var handler = new GetByIdHandler<PhysicalStructureAgg, PhysicalStructureDto>(_readOnlyServiceMock.Object);
        var id = Guid.NewGuid();
        var dto = new PhysicalStructureDto { Id = id, Name = "Estructura Base" };
        var query = new GetByIdQuery<PhysicalStructureAgg, PhysicalStructureDto>(id);

        _readOnlyServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(dto);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().Be(dto);
        _readOnlyServiceMock.Verify(s => s.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetAllHandler_ShouldCallGetAllAsync_AndReturnListOfDtos()
    {
        // Arrange
        var handler = new GetAllHandler<PhysicalStructureAgg, PhysicalStructureDto>(_readOnlyServiceMock.Object);
        var dtoList = new List<PhysicalStructureDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Edificio A" },
            new() { Id = Guid.NewGuid(), Name = "Edificio B" }
        };
        var query = new GetAllQuery<PhysicalStructureAgg, PhysicalStructureDto>();

        _readOnlyServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(dtoList);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(dtoList);
        _readOnlyServiceMock.Verify(s => s.GetAllAsync(), Times.Once);
    }
}
