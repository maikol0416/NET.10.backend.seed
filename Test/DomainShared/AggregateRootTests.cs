using Domain.DomainShared;
using Domain.Ports.Events.Properties;

namespace Test.DomainShared;

/// <summary>
/// Pruebas unitarias para la clase base AggregateRoot.
/// Verifica el ciclo de vida de los Domain Events: registro, colección y limpieza.
/// </summary>
public class AggregateRootTests
{
    // ─────────────────────────────────────────────
    // Stubs mínimos para poder instanciar AggregateRoot (es abstract)
    // ─────────────────────────────────────────────

    /// <summary>Evento de dominio de prueba.</summary>
    private sealed class StubDomainEvent : IDomainEvent { }

    /// <summary>Aggregate Root concreto mínimo sin invariantes de negocio.</summary>
    private sealed class StubAggregateRoot : AggregateRoot
    {
        public StubAggregateRoot() : base() { }

        protected override void ExcecuteDomainInvariants() { /* sin reglas en el stub */ }

        /// <summary>Expone AddDomainEvent para facilitar las pruebas.</summary>
        public void RegisterEvent(IDomainEvent domainEvent) => AddDomainEvent(domainEvent);
    }

    // ─────────────────────────────────────────────
    // Estado inicial
    // ─────────────────────────────────────────────

    [Fact]
    public void DomainEvents_WhenAggregateIsCreated_ShouldBeEmpty()
    {
        // Arrange & Act
        var aggregate = new StubAggregateRoot();

        // Assert
        aggregate.DomainEvents.Should().BeEmpty(
            "un agregado recién creado no debe tener eventos pendientes.");
    }

    // ─────────────────────────────────────────────
    // AddDomainEvent
    // ─────────────────────────────────────────────

    [Fact]
    public void AddDomainEvent_WhenEventIsRegistered_ShouldAppearInDomainEvents()
    {
        // Arrange
        var aggregate = new StubAggregateRoot();
        var domainEvent = new StubDomainEvent();

        // Act
        aggregate.RegisterEvent(domainEvent);

        // Assert
        aggregate.DomainEvents.Should().ContainSingle(
            "se registró exactamente un evento.")
            .Which.Should().BeSameAs(domainEvent);
    }

    [Fact]
    public void AddDomainEvent_WhenMultipleEventsAreRegistered_ShouldPreserveOrder()
    {
        // Arrange
        var aggregate = new StubAggregateRoot();
        var event1 = new StubDomainEvent();
        var event2 = new StubDomainEvent();

        // Act
        aggregate.RegisterEvent(event1);
        aggregate.RegisterEvent(event2);

        // Assert
        aggregate.DomainEvents.Should().HaveCount(2);
        aggregate.DomainEvents.Should()
            .ContainInOrder(new IDomainEvent[] { event1, event2 },
                "los eventos deben respetarse en orden de registro (FIFO).");
    }

    // ─────────────────────────────────────────────
    // ClearDomainEvents
    // ─────────────────────────────────────────────

    [Fact]
    public void ClearDomainEvents_WhenCalled_ShouldRemoveAllEvents()
    {
        // Arrange
        var aggregate = new StubAggregateRoot();
        aggregate.RegisterEvent(new StubDomainEvent());
        aggregate.RegisterEvent(new StubDomainEvent());

        // Act
        aggregate.ClearDomainEvents();

        // Assert
        aggregate.DomainEvents.Should().BeEmpty(
            "después de limpiar, la colección de eventos debe estar vacía.");
    }

    // ─────────────────────────────────────────────
    // Colección de solo lectura
    // ─────────────────────────────────────────────

    [Fact]
    public void DomainEvents_ExposedCollection_ShouldBeReadOnly()
    {
        // Arrange & Act
        var aggregate = new StubAggregateRoot();

        // Assert
        aggregate.DomainEvents.Should().BeAssignableTo<IReadOnlyCollection<IDomainEvent>>(
            "DomainEvents debe ser de solo lectura para proteger el estado interno del agregado.");
    }
}
