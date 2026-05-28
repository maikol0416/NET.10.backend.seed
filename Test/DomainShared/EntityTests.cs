using Domain.DomainShared;

namespace Test.DomainShared;

/// <summary>
/// Pruebas unitarias para la clase base Entity.
/// Verifica que toda entidad nazca con un estado válido y consistente.
/// </summary>
public class EntityTests
{
    // ─────────────────────────────────────────────
    // Clase concreta mínima para poder instanciar Entity (es abstract)
    // ─────────────────────────────────────────────
    private sealed class StubEntity : Entity
    {
        public StubEntity() : base() { }
    }

    // ─────────────────────────────────────────────
    // Id
    // ─────────────────────────────────────────────

    [Fact]
    public void Constructor_WhenEntityIsCreated_IdShouldBeAssigned()
    {
        // Arrange & Act
        var entity = new StubEntity();

        // Assert
        entity.Id.Should().NotBeEmpty("toda entidad debe recibir un Id en construcción.");
    }

    [Fact]
    public void Constructor_WhenEntityIsCreated_IdShouldBeUniquePerInstance()
    {
        // Arrange & Act
        var entity1 = new StubEntity();
        var entity2 = new StubEntity();

        // Assert
        entity1.Id.Should().NotBe(entity2.Id,
            "cada instancia de Entity debe tener un Id único.");
    }

    // ─────────────────────────────────────────────
    // Status
    // ─────────────────────────────────────────────

    [Fact]
    public void Constructor_WhenEntityIsCreated_StatusShouldBeActive()
    {
        // Arrange & Act
        var entity = new StubEntity();

        // Assert
        entity.Status.Should().Be("1",
            "el estado inicial por convención del dominio es '1' (activo).");
    }

    // ─────────────────────────────────────────────
    // CreatedAt
    // ─────────────────────────────────────────────

    [Fact]
    public void Constructor_WhenEntityIsCreated_CreatedAtShouldBeSetToUtcNow()
    {
        // Arrange
        var before = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var entity = new StubEntity();

        // Assert
        var after = DateTime.UtcNow.AddSeconds(1);
        entity.CreatedAt.Should().BeAfter(before)
              .And.BeBefore(after,
              "CreatedAt debe reflejar el momento de construcción en UTC.");
    }

    // ─────────────────────────────────────────────
    // UpdateAt
    // ─────────────────────────────────────────────

    [Fact]
    public void Constructor_WhenEntityIsCreated_UpdateAtShouldBeNull()
    {
        // Arrange & Act
        var entity = new StubEntity();

        // Assert
        entity.UpdateAt.Should().BeNull(
            "una entidad recién creada no ha sido actualizada.");
    }
}
