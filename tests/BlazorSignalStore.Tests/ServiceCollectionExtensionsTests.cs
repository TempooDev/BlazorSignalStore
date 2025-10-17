using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using BlazorSignalStore.Core;

namespace BlazorSignalStore.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddSignalStore_ShouldRegisterStoreAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSignalStore<TestStore>();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var store1 = serviceProvider.GetService<TestStore>();
        var store2 = serviceProvider.GetService<TestStore>();

        store1.Should().NotBeNull();
        store2.Should().NotBeNull();
        store1.Should().BeSameAs(store2); // Should be same instance (scoped)
    }

    [Fact]
    public void AddSignalStore_ShouldAllowMultipleStores()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSignalStore<TestStore>();
        services.AddSignalStore<AnotherTestStore>();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var store1 = serviceProvider.GetService<TestStore>();
        var store2 = serviceProvider.GetService<AnotherTestStore>();

        store1.Should().NotBeNull();
        store2.Should().NotBeNull();
        store1.Should().NotBeSameAs(store2);
    }

    [Fact]
    public void AddSignalStore_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddSignalStore<TestStore>();

        // Assert
        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddSignalStore_WithStoreBase_ShouldWork()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSignalStore<StoreBaseDerived>();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var store = serviceProvider.GetService<StoreBaseDerived>();
        store.Should().NotBeNull();
        store.Should().BeAssignableTo<StoreBase>();
    }

    [Fact]
    public void AddSignalStore_MultipleScopesScenario_ShouldCreateDifferentInstances()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSignalStore<TestStore>();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        TestStore? store1, store2, store3, store4;

        using (var scope1 = serviceProvider.CreateScope())
        {
            store1 = scope1.ServiceProvider.GetService<TestStore>();
            store2 = scope1.ServiceProvider.GetService<TestStore>();
        }

        using (var scope2 = serviceProvider.CreateScope())
        {
            store3 = scope2.ServiceProvider.GetService<TestStore>();
            store4 = scope2.ServiceProvider.GetService<TestStore>();
        }

        // Assert
        store1.Should().NotBeNull();
        store2.Should().NotBeNull();
        store3.Should().NotBeNull();
        store4.Should().NotBeNull();

        // Same within scope
        store1.Should().BeSameAs(store2);
        store3.Should().BeSameAs(store4);

        // Different across scopes
        store1.Should().NotBeSameAs(store3);
    }
}

// Test classes
public class TestStore
{
    public Signal<int> Counter { get; } = new(0);
    public Signal<string> Name { get; } = new("Test");

    public void IncrementCounter() => Counter.Value++;
    public void SetName(string name) => Name.Value = name;
}

public class AnotherTestStore
{
    public Signal<bool> IsActive { get; } = new(false);

    public void Toggle() => IsActive.Value = !IsActive.Value;
}

public class StoreBaseDerived : StoreBase
{
    public Signal<DateTime> LastUpdated { get; } = new(DateTime.Now);

    public void UpdateTimestamp() => LastUpdated.Value = DateTime.Now;
}
