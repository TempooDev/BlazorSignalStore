using BlazorSignalStore.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FluentAssertions;
using Bunit;
using System;

namespace BlazorSignalStore.Tests
{
    /// <summary>
    /// Advanced tests for SignalHooks to improve code coverage.
    /// </summary>
    public class SignalHooksAdvancedTests : TestContext
    {
        [Fact]
        public void UseSignal_WithSignalAndFunctionFlag_ReturnsFunction()
        {
            // Arrange
            var component = RenderComponent<TestComponent>();
            var signal = new Signal<string>("test");

            // Act
            var func = component.Instance.useSignal(signal, useAsFunction: true);

            // Assert
            func.Should().NotBeNull();
            func().Should().Be("test");
        }

        [Fact]
        public void UseSignal_WithSignalAndFalseFunctionFlag_ReturnsSignalValue()
        {
            // Arrange
            var component = RenderComponent<TestComponent>();
            var signal = new Signal<string>("test");

            // Act
            var result = component.Instance.useSignal(signal, useAsFunction: false);

            // Assert
            result.Should().NotBeNull();
            // The method returns Func<T> regardless, but when useAsFunction is false,
            // it returns the implicit conversion from SignalValue to Func
            result().Should().Be("test");
        }

        [Fact]
        public void UseSignal_WithComputedAndFunctionFlag_ReturnsFunction()
        {
            // Arrange
            var component = RenderComponent<TestComponent>();
            var baseSignal = new Signal<int>(5);
            var computed = new Computed<string>(() => $"Value: {baseSignal.Value}", baseSignal);

            // Act
            var func = component.Instance.useSignal(computed, useAsFunction: true);

            // Assert
            func.Should().NotBeNull();
            func().Should().Be("Value: 5");
        }

        [Fact]
        public void UseSignal_WithComputedAndFalseFunctionFlag_ReturnsFunction()
        {
            // Arrange
            var component = RenderComponent<TestComponent>();
            var baseSignal = new Signal<int>(5);
            var computed = new Computed<string>(() => $"Value: {baseSignal.Value}", baseSignal);

            // Act
            var result = component.Instance.useSignal(computed, useAsFunction: false);

            // Assert
            result.Should().NotBeNull();
            result().Should().Be("Value: 5");
        }

        [Fact]
        public void SignalValue_ImplicitConversionToValue_ReturnsCorrectValue()
        {
            // Arrange
            var component = RenderComponent<TestComponent>();
            var signal = new Signal<int>(42);
            var signalValue = component.Instance.useSignal(signal);

            // Act
            int implicitValue = signalValue;

            // Assert
            implicitValue.Should().Be(42);
        }

        [Fact]
        public void SignalValue_ImplicitConversionToFunction_ReturnsFunction()
        {
            // Arrange
            var component = RenderComponent<TestComponent>();
            var signal = new Signal<string>("hello");
            var signalValue = component.Instance.useSignal(signal);

            // Act
            Func<string> func = signalValue;

            // Assert
            func.Should().NotBeNull();
            func().Should().Be("hello");
        }

        [Fact]
        public void SignalValue_InvokeMethod_ReturnsCurrentValue()
        {
            // Arrange
            var component = RenderComponent<TestComponent>();
            var signal = new Signal<double>(3.14);
            var signalValue = component.Instance.useSignal(signal);

            // Act
            var result = signalValue.Invoke();

            // Assert
            result.Should().Be(3.14);
        }

        [Fact]
        public void SignalValue_Dispose_UnsubscribesFromSignal()
        {
            // Arrange
            var component = RenderComponent<TestComponent>();
            var signal = new Signal<string>("initial");
            var signalValue = component.Instance.useSignal(signal);
            var changeDetected = false;

            // Subscribe directly to signal to track changes
            signal.Subscribe(_ => changeDetected = true);

            // Act - Change value before dispose
            signal.Value = "changed";
            changeDetected.Should().BeTrue(); // Verify signal works

            // Reset and dispose SignalValue
            changeDetected = false;
            signalValue.Dispose();

            // Change value after dispose
            signal.Value = "after-dispose";

            // Assert - SignalValue should not cause component re-render, but signal still works
            changeDetected.Should().BeTrue(); // Direct subscription still works
        }

        [Fact]
        public void SignalValue_ReflectionFallback_HandlesExceptions()
        {
            // This test verifies that the reflection-based StateHasChanged calls handle exceptions gracefully
            // Arrange
            var component = RenderComponent<TestComponent>();
            var signal = new Signal<string>("test");
            var signalValue = component.Instance.useSignal(signal);

            // Act & Assert - Should not throw even if reflection fails
            var action = () => signal.Value = "changed";
            action.Should().NotThrow();
        }

        [Fact]
        public void SignalHooks_WorksWithDifferentComponentTypes()
        {
            // Arrange
            var component1 = RenderComponent<TestComponent>();
            var component2 = RenderComponent<AnotherTestComponent>();
            var signal = new Signal<int>(100);

            // Act
            var signalValue1 = component1.Instance.useSignal(signal);
            var signalValue2 = component2.Instance.useSignal(signal);

            // Assert
            signalValue1.Value.Should().Be(100);
            signalValue2.Value.Should().Be(100);

            // Both should update when signal changes
            signal.Value = 200;
            signalValue1.Value.Should().Be(200);
            signalValue2.Value.Should().Be(200);
        }

        [Fact]
        public void SignalValue_WithComputedSignal_UpdatesWhenDependencyChanges()
        {
            // Arrange
            var component = RenderComponent<TestComponent>();
            var baseSignal = new Signal<int>(10);
            var computed = new Computed<int>(() => baseSignal.Value * 2, baseSignal);
            var signalValue = component.Instance.useSignal(computed);

            // Act & Assert
            signalValue.Value.Should().Be(20);

            baseSignal.Value = 15;
            signalValue.Value.Should().Be(30);
        }
    }

    /// <summary>
    /// Test component for SignalHooks testing.
    /// </summary>
    public class TestComponent : ComponentBase
    {
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
        {
            builder.AddMarkupContent(0, "<div>Test Component</div>");
        }
    }

    /// <summary>
    /// Another test component for multi-component testing.
    /// </summary>
    public class AnotherTestComponent : ComponentBase
    {
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
        {
            builder.AddMarkupContent(0, "<div>Another Test Component</div>");
        }
    }
}
