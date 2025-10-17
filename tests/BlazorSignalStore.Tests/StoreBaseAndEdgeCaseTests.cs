using BlazorSignalStore.Core;
using Xunit;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorSignalStore.Tests
{
    /// <summary>
    /// Tests for StoreBase and additional edge cases.
    /// </summary>
    public class StoreBaseTests
    {
        [Fact]
        public void StoreBase_CanBeInherited_AndWorksCorrectly()
        {
            // Arrange & Act
            var store = new CounterTestStore();

            // Assert
            store.Should().NotBeNull();
            store.Counter.Value.Should().Be(0);
            store.DisplayText.Value.Should().Be("Count: 0");
        }

        [Fact]
        public void StoreBase_MultipleInstances_AreIndependent()
        {
            // Arrange
            var store1 = new CounterTestStore();
            var store2 = new CounterTestStore();

            // Act
            store1.Increment();
            store1.Increment();

            // Assert
            store1.Counter.Value.Should().Be(2);
            store2.Counter.Value.Should().Be(0); // Should remain independent
        }

        [Fact]
        public void StoreBase_ComplexOperations_WorkCorrectly()
        {
            // Arrange
            var store = new CounterTestStore();

            // Act
            store.IncrementBy(5);
            store.IncrementBy(3);

            // Assert
            store.Counter.Value.Should().Be(8);
            store.DisplayText.Value.Should().Be("Count: 8");
        }

        [Fact]
        public void StoreBase_ResetOperation_WorksCorrectly()
        {
            // Arrange
            var store = new CounterTestStore();
            store.IncrementBy(10);

            // Act
            store.Reset();

            // Assert
            store.Counter.Value.Should().Be(0);
            store.DisplayText.Value.Should().Be("Count: 0");
        }
    }

    /// <summary>
    /// Test store implementation for testing StoreBase functionality.
    /// </summary>
    public class CounterTestStore : StoreBase
    {
        public Signal<int> Counter { get; } = new(0);
        public Computed<string> DisplayText { get; }

        public CounterTestStore()
        {
            DisplayText = new Computed<string>(() => $"Count: {Counter.Value}", Counter);
        }

        public void Increment() => Counter.Value++;

        public void IncrementBy(int amount) => Counter.Value += amount;

        public void Reset() => Counter.Value = 0;
    }

    /// <summary>
    /// Tests for edge cases and error conditions.
    /// </summary>
    public class EdgeCaseTests
    {
        [Fact]
        public void Signal_WithNullValue_HandlesCorrectly()
        {
            // Arrange & Act
            var signal = new Signal<string?>(null);

            // Assert
            signal.Value.Should().BeNull();
        }

        [Fact]
        public void Signal_WithLargeObject_HandlesCorrectly()
        {
            // Arrange
            var largeData = new LargeTestData
            {
                Data = new string('A', 10000),
                Numbers = Enumerable.Range(1, 1000).ToArray()
            };

            // Act
            var signal = new Signal<LargeTestData>(largeData);

            // Assert
            signal.Value.Should().NotBeNull();
            signal.Value.Data.Should().HaveLength(10000);
            signal.Value.Numbers.Should().HaveCount(1000);
        }

        [Fact]
        public void Computed_WithNullDependency_HandlesCorrectly()
        {
            // Arrange
            var nullSignal = new Signal<string?>(null);
            var computed = new Computed<string>(() =>
                nullSignal.Value ?? "Default", nullSignal);

            // Act & Assert
            computed.Value.Should().Be("Default");

            nullSignal.Value = "Not null";
            computed.Value.Should().Be("Not null");

            nullSignal.Value = null;
            computed.Value.Should().Be("Default");
        }

        [Fact]
        public void Signal_RapidUpdates_HandlesCorrectly()
        {
            // Arrange
            var signal = new Signal<int>(0);
            var notifications = new List<int>();
            var subscription = signal.Subscribe(value => notifications.Add(value));

            // Act - Rapid updates
            for (int i = 1; i <= 100; i++)
            {
                signal.Value = i;
            }

            // Assert - Signal should handle all updates (initial value + 100 changes = 101 total)
            signal.Value.Should().Be(100);
            notifications.Should().HaveCount(101); // Initial subscription notification + 100 updates
            notifications.Last().Should().Be(100);

            // Cleanup
            subscription.Dispose();
        }

        [Fact]
        public void Computed_WithRecursiveCalculation_HandlesCorrectly()
        {
            // Arrange
            var signal = new Signal<int>(5);
            var computed = new Computed<int>(() => Fibonacci(signal.Value), signal);

            // Act & Assert
            computed.Value.Should().Be(5); // Fibonacci(5) = 5

            signal.Value = 7;
            computed.Value.Should().Be(13); // Fibonacci(7) = 13
        }

        private int Fibonacci(int n)
        {
            if (n <= 1) return n;
            return Fibonacci(n - 1) + Fibonacci(n - 2);
        }
    }

    /// <summary>
    /// Large test data for testing memory and performance.
    /// </summary>
    public class LargeTestData
    {
        public string Data { get; set; } = string.Empty;
        public int[] Numbers { get; set; } = Array.Empty<int>();
    }
}
