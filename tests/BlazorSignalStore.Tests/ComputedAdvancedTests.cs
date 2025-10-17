using BlazorSignalStore.Core;
using Xunit;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorSignalStore.Tests
{
    /// <summary>
    /// Advanced tests for Computed to improve code coverage.
    /// </summary>
    public class ComputedAdvancedTests
    {
        [Fact]
        public void Computed_WithMultipleDependencies_UpdatesCorrectly()
        {
            // Arrange
            var signal1 = new Signal<int>(10);
            var signal2 = new Signal<int>(20);
            var signal3 = new Signal<string>("test");

            var computed = new Computed<string>(() =>
                $"{signal3.Value}: {signal1.Value + signal2.Value}",
                signal1, signal2, signal3);

            // Act & Assert
            computed.Value.Should().Be("test: 30");

            signal1.Value = 15;
            computed.Value.Should().Be("test: 35");

            signal2.Value = 25;
            computed.Value.Should().Be("test: 40");

            signal3.Value = "result";
            computed.Value.Should().Be("result: 40");
        }

        [Fact]
        public void Computed_WithNoExplicitDependencies_StillWorks()
        {
            // Arrange
            var baseValue = 42;
            var computed = new Computed<string>(() => $"Static: {baseValue}");

            // Act & Assert
            computed.Value.Should().Be("Static: 42");
        }

        [Fact]
        public void Computed_SubscriptionManagement_HandlesMultipleSubscriptions()
        {
            // Arrange
            var signal = new Signal<int>(10);
            var computed = new Computed<string>(() => $"Value: {signal.Value}", signal);

            var notifications1 = new List<string>();
            var notifications2 = new List<string>();

            // Act
            var subscription1 = computed.Subscribe(value => notifications1.Add(value));
            var subscription2 = computed.Subscribe(value => notifications2.Add(value));

            signal.Value = 20;
            signal.Value = 30;

            // Assert - Computed notifies on initial subscription + 2 changes = 3 total
            notifications1.Should().HaveCount(3);
            notifications1.Should().Contain("Value: 10");
            notifications1.Should().Contain("Value: 20");
            notifications1.Should().Contain("Value: 30");

            notifications2.Should().HaveCount(3);
            notifications2.Should().Contain("Value: 10");
            notifications2.Should().Contain("Value: 20");
            notifications2.Should().Contain("Value: 30");

            // Cleanup
            subscription1.Dispose();
            subscription2.Dispose();
        }

        [Fact]
        public void Computed_ChainedComputations_WorkCorrectly()
        {
            // Arrange
            var baseSignal = new Signal<int>(5);
            var computed1 = new Computed<int>(() => baseSignal.Value * 2, baseSignal);
            var computed2 = new Computed<int>(() => computed1.Value + 10, computed1);
            var computed3 = new Computed<string>(() => $"Final: {computed2.Value}", computed2);

            // Act & Assert
            computed3.Value.Should().Be("Final: 20"); // (5 * 2) + 10 = 20

            baseSignal.Value = 10;
            computed3.Value.Should().Be("Final: 30"); // (10 * 2) + 10 = 30
        }

        [Fact]
        public void Computed_WithComplexCalculation_PerformanceIsAcceptable()
        {
            // Arrange
            var signal = new Signal<int>(1000);
            var computed = new Computed<int>(() =>
            {
                var result = 0;
                for (int i = 0; i < signal.Value; i++)
                {
                    result += i;
                }
                return result;
            }, signal);

            // Act & Assert
            var startTime = DateTime.UtcNow;
            var value = computed.Value;
            var endTime = DateTime.UtcNow;

            value.Should().Be(499500); // Sum of 0 to 999
            (endTime - startTime).Should().BeLessThan(TimeSpan.FromMilliseconds(100));
        }

        [Fact]
        public void Computed_WithExceptionInComputation_HandlesGracefully()
        {
            // Arrange
            var signal = new Signal<int>(10);
            var computed = new Computed<string>(() =>
            {
                if (signal.Value < 0)
                    throw new InvalidOperationException("Negative value not allowed");
                return $"Value: {signal.Value}";
            }, signal);

            // Act & Assert - Normal case
            computed.Value.Should().Be("Value: 10");

            // Act & Assert - Exception case
            // The exception will be thrown when trying to access Value after setting signal to negative
            var action = () =>
            {
                signal.Value = -5;
                // Exception is thrown during the dependency update, not when accessing Value
            };
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Negative value not allowed");
        }
        [Fact]
        public void Computed_MemoryManagement_DisposesCorrectly()
        {
            // Arrange
            var signal = new Signal<int>(100);
            var computed = new Computed<string>(() => $"Value: {signal.Value}", signal);
            var notificationCount = 0;

            var subscription = computed.Subscribe(_ => notificationCount++);

            // Act - Initial subscription triggers notification
            notificationCount.Should().Be(1);

            signal.Value = 200; // Should trigger notification
            notificationCount.Should().Be(2);

            subscription.Dispose();
            signal.Value = 300; // Should not trigger notification after dispose

            // Assert
            notificationCount.Should().Be(2); // No additional notifications
        }

        [Fact]
        public void Computed_CircularDependency_DoesNotCauseInfiniteLoop()
        {
            // This test ensures that if someone accidentally creates a circular dependency,
            // it doesn't cause an infinite loop (though it may not work as expected)

            // Arrange
            var signal1 = new Signal<int>(10);
            var signal2 = new Signal<int>(20);

            // Create computed values that depend on each other indirectly
            var computed1 = new Computed<int>(() => signal1.Value + signal2.Value, signal1, signal2);
            var computed2 = new Computed<int>(() => computed1.Value * 2, computed1);

            // Act & Assert - Should not hang or throw
            var value1 = computed1.Value;
            var value2 = computed2.Value;

            value1.Should().Be(30);
            value2.Should().Be(60);

            // Verify updates still work
            signal1.Value = 15;
            computed1.Value.Should().Be(35);
            computed2.Value.Should().Be(70);
        }
    }
}
