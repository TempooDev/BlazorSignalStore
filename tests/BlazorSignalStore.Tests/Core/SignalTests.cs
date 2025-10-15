using FluentAssertions;
using BlazorSignalStore.Core;

namespace BlazorSignalStore.Tests.Core;

public class SignalTests
{
    [Fact]
    public void Signal_ShouldInitializeWithValue()
    {
        // Arrange & Act
        var signal = new Signal<int>(42);

        // Assert
        signal.Value.Should().Be(42);
    }

    [Fact]
    public void Signal_ShouldNotifyOnValueChange()
    {
        // Arrange
        var signal = new Signal<int>(0);
        var changeNotifications = new List<int>();
        signal.Changed += value => changeNotifications.Add(value);

        // Act
        signal.Value = 10;
        signal.Value = 20;

        // Assert
        changeNotifications.Should().HaveCount(2);
        changeNotifications[0].Should().Be(10);
        changeNotifications[1].Should().Be(20);
    }

    [Fact]
    public void Signal_ShouldNotNotifyOnSameValue()
    {
        // Arrange
        var signal = new Signal<int>(42);
        var changeCount = 0;
        signal.Changed += _ => changeCount++;

        // Act
        signal.Value = 42; // Same value
        signal.Value = 42; // Same value again

        // Assert
        changeCount.Should().Be(0);
    }

    [Fact]
    public void Signal_ShouldNotifyOnValueChangeFromSameToSame()
    {
        // Arrange
        var signal = new Signal<int>(42);
        var changeCount = 0;
        signal.Changed += _ => changeCount++;

        // Act
        signal.Value = 10; // Different value
        signal.Value = 10; // Same value as previous

        // Assert
        changeCount.Should().Be(1); // Only one notification for the actual change
    }

    [Fact]
    public void Signal_ImplicitCast_ShouldReturnValue()
    {
        // Arrange
        var signal = new Signal<string>("Hello");

        // Act
        string value = signal; // Implicit cast

        // Assert
        value.Should().Be("Hello");
    }

    [Fact]
    public void Signal_Invoke_ShouldReturnValue()
    {
        // Arrange
        var signal = new Signal<double>(3.14);

        // Act
        var value = signal.Invoke();

        // Assert
        value.Should().Be(3.14);
    }

    [Fact]
    public void Signal_Subscribe_ShouldCallListenerImmediately()
    {
        // Arrange
        var signal = new Signal<string>("initial");
        var receivedValues = new List<string>();

        // Act
        var subscription = signal.Subscribe(value => receivedValues.Add(value));

        // Assert
        receivedValues.Should().HaveCount(1);
        receivedValues[0].Should().Be("initial");
    }

    [Fact]
    public void Signal_Subscribe_ShouldCallListenerOnChanges()
    {
        // Arrange
        var signal = new Signal<int>(1);
        var receivedValues = new List<int>();
        var subscription = signal.Subscribe(value => receivedValues.Add(value));

        // Act
        signal.Value = 2;
        signal.Value = 3;

        // Assert
        receivedValues.Should().HaveCount(3); // Initial + 2 changes
        receivedValues.Should().Equal(1, 2, 3);
    }

    [Fact]
    public void Signal_Unsubscribe_ShouldStopNotifications()
    {
        // Arrange
        var signal = new Signal<int>(1);
        var receivedValues = new List<int>();
        var subscription = signal.Subscribe(value => receivedValues.Add(value));

        // Act
        subscription.Dispose(); // Unsubscribe
        signal.Value = 2;
        signal.Value = 3;

        // Assert
        receivedValues.Should().HaveCount(1); // Only the initial value
        receivedValues[0].Should().Be(1);
    }

    [Fact]
    public void Signal_WithNullableType_ShouldHandleNullValues()
    {
        // Arrange
        var signal = new Signal<string?>(null);
        var changeNotifications = new List<string?>();
        signal.Changed += value => changeNotifications.Add(value);

        // Act
        signal.Value = "hello";
        signal.Value = null;

        // Assert
        changeNotifications.Should().HaveCount(2);
        changeNotifications[0].Should().Be("hello");
        changeNotifications[1].Should().BeNull();
    }

    [Fact]
    public void Signal_WithComplexType_ShouldDetectChanges()
    {
        // Arrange
        var person1 = new Person("John", 30);
        var person2 = new Person("Jane", 25);
        var signal = new Signal<Person>(person1);
        var changeNotifications = new List<Person>();
        signal.Changed += value => changeNotifications.Add(value);

        // Act
        signal.Value = person2;
        signal.Value = person1; // Back to original, but different instance

        // Assert
        changeNotifications.Should().HaveCount(2);
        changeNotifications[0].Should().Be(person2);
        changeNotifications[1].Should().Be(person1);
    }

    private record Person(string Name, int Age);
}