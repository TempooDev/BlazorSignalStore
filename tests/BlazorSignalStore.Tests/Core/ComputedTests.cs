using FluentAssertions;
using BlazorSignalStore.Core;

namespace BlazorSignalStore.Tests.Core;

public class ComputedTests
{
    [Fact]
    public void Computed_ShouldCalculateInitialValue()
    {
        // Arrange
        var baseSignal = new Signal<int>(10);
        var computed = new Computed<int>(() => baseSignal.Value * 2, baseSignal);

        // Act & Assert
        computed.Value.Should().Be(20);
    }

    [Fact]
    public void Computed_ShouldRecalculateWhenDependencyChanges()
    {
        // Arrange
        var baseSignal = new Signal<int>(5);
        var computed = new Computed<int>(() => baseSignal.Value * 3, baseSignal);
        var changeNotifications = new List<int>();
        computed.Changed += value => changeNotifications.Add(value);

        // Act
        baseSignal.Value = 10; // Should trigger recalculation
        baseSignal.Value = 7;  // Should trigger another recalculation

        // Assert
        computed.Value.Should().Be(21); // 7 * 3
        changeNotifications.Should().HaveCount(2);
        changeNotifications[0].Should().Be(30); // 10 * 3
        changeNotifications[1].Should().Be(21); // 7 * 3
    }

    [Fact]
    public void Computed_WithMultipleDependencies_ShouldRecalculateOnAnyChange()
    {
        // Arrange
        var signal1 = new Signal<int>(2);
        var signal2 = new Signal<int>(3);
        var computed = new Computed<int>(() => signal1.Value + signal2.Value, signal1, signal2);
        var changeNotifications = new List<int>();
        computed.Changed += value => changeNotifications.Add(value);

        // Act
        signal1.Value = 5; // 5 + 3 = 8
        signal2.Value = 7; // 5 + 7 = 12

        // Assert
        computed.Value.Should().Be(12);
        changeNotifications.Should().HaveCount(2);
        changeNotifications[0].Should().Be(8);
        changeNotifications[1].Should().Be(12);
    }

    [Fact]
    public void Computed_WithStringConcatenation_ShouldWork()
    {
        // Arrange
        var firstName = new Signal<string>("John");
        var lastName = new Signal<string>("Doe");
        var fullName = new Computed<string>(() => $"{firstName.Value} {lastName.Value}", firstName, lastName);

        // Act & Assert
        fullName.Value.Should().Be("John Doe");

        // Change first name
        firstName.Value = "Jane";
        fullName.Value.Should().Be("Jane Doe");

        // Change last name
        lastName.Value = "Smith";
        fullName.Value.Should().Be("Jane Smith");
    }

    [Fact]
    public void Computed_NestedComputed_ShouldWork()
    {
        // Arrange
        var baseValue = new Signal<int>(10);
        var doubled = new Computed<int>(() => baseValue.Value * 2, baseValue);
        var quadrupled = new Computed<int>(() => doubled.Value * 2, doubled);

        // Act & Assert
        quadrupled.Value.Should().Be(40); // 10 * 2 * 2

        // Change base value
        baseValue.Value = 5;
        quadrupled.Value.Should().Be(20); // 5 * 2 * 2
    }

    [Fact]
    public void Computed_ShouldInheritFromSignal()
    {
        // Arrange
        var baseSignal = new Signal<int>(100);
        var computed = new Computed<string>(() => $"Value: {baseSignal.Value}", baseSignal);

        // Act & Assert
        computed.Should().BeAssignableTo<Signal<string>>();

        // Test Signal methods
        computed.Invoke().Should().Be("Value: 100");

        // Test implicit cast
        string value = computed;
        value.Should().Be("Value: 100");
    }

    [Fact]
    public void Computed_Subscribe_ShouldWorkLikeSignal()
    {
        // Arrange
        var baseSignal = new Signal<int>(1);
        var computed = new Computed<string>(() => $"Count: {baseSignal.Value}", baseSignal);
        var receivedValues = new List<string>();

        // Act
        var subscription = computed.Subscribe(value => receivedValues.Add(value));
        baseSignal.Value = 2;
        baseSignal.Value = 3;

        // Assert
        receivedValues.Should().HaveCount(3); // Initial + 2 changes
        receivedValues[0].Should().Be("Count: 1");
        receivedValues[1].Should().Be("Count: 2");
        receivedValues[2].Should().Be("Count: 3");

        // Cleanup
        subscription.Dispose();
    }

    [Fact]
    public void Computed_WithComplexCalculation_ShouldWork()
    {
        // Arrange
        var radius = new Signal<double>(5.0);
        var area = new Computed<double>(() => Math.PI * radius.Value * radius.Value, radius);
        var changeNotifications = new List<double>();
        area.Changed += value => changeNotifications.Add(value);

        // Act
        radius.Value = 3.0;

        // Assert
        area.Value.Should().BeApproximately(Math.PI * 9, 0.001);
        changeNotifications.Should().HaveCount(1);
        changeNotifications[0].Should().BeApproximately(Math.PI * 9, 0.001);
    }

    [Fact]
    public void Computed_ShouldNotRecalculateOnSameDependencyValue()
    {
        // Arrange
        var baseSignal = new Signal<int>(10);
        var computed = new Computed<int>(() => baseSignal.Value * 2, baseSignal);
        var changeCount = 0;
        computed.Changed += _ => changeCount++;

        // Act
        baseSignal.Value = 10; // Same value, should not trigger recalculation
        baseSignal.Value = 15; // Different value, should trigger
        baseSignal.Value = 15; // Same value again, should not trigger

        // Assert
        changeCount.Should().Be(1); // Only one actual change
        computed.Value.Should().Be(30); // 15 * 2
    }
}
