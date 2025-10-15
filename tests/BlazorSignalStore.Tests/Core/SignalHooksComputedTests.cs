using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using BlazorSignalStore.Core;
using Xunit;

namespace BlazorSignalStore.Tests.Core;

/// <summary>
/// Tests for the new Computed&lt;T&gt; support in SignalHooks.
/// </summary>
public class SignalHooksComputedTests : TestContext
{
    [Fact]
    public void UseSignal_WithComputed_ShouldReturnSignalValue()
    {
        // Arrange
        var component = RenderComponent<TestComponentWithComputed>();
        var baseSignal = new Signal<int>(42);
        var computed = new Computed<string>(() => $"Value: {baseSignal.Value}", baseSignal);

        // Act
        var signalValue = component.Instance.TestUseSignal(computed);

        // Assert
        signalValue.Should().NotBeNull();
        signalValue.Value.Should().Be("Value: 42");
    }

    [Fact]
    public void UseSignal_WithComputed_ShouldTriggerRerender()
    {
        // Arrange
        var baseSignal = new Signal<int>(10);
        var computed = new Computed<string>(() => $"Count: {baseSignal.Value}", baseSignal);
        Services.AddSingleton(baseSignal);
        Services.AddSingleton(computed);
        var component = RenderComponent<ComputedComponent>();

        // Act
        baseSignal.Value = 25;

        // Assert
        // The component should have re-rendered with the new computed value
        component.Find("p").TextContent.Should().Be("Count: 25");
    }

    [Fact]
    public void UseSignal_WithComputedAndFunctionFlag_ShouldReturnFunction()
    {
        // Arrange
        var component = RenderComponent<TestComponentWithComputed>();
        var baseSignal = new Signal<double>(3.14);
        var computed = new Computed<string>(() => $"Pi: {baseSignal.Value:F2}", baseSignal);

        // Act
        var func = component.Instance.TestUseSignalAsFunction(computed);

        // Assert
        func.Should().NotBeNull();
        func().Should().Be("Pi: 3.14");
    }

    [Fact]
    public void UseSignal_WithComputedAndFalseFunctionFlag_ShouldReturnSignalValue()
    {
        // Arrange
        var component = RenderComponent<TestComponentWithComputed>();
        var baseSignal = new Signal<bool>(true);
        var computed = new Computed<string>(() => baseSignal.Value ? "Yes" : "No", baseSignal);

        // Act
        var result = component.Instance.TestUseSignalWithFlag(computed, false);

        // Assert
        result.Should().NotBeNull();
        result().Should().Be("Yes");
    }

    [Fact]
    public void SignalValue_WithComputed_ImplicitConversionToValue_ShouldWork()
    {
        // Arrange
        var component = RenderComponent<TestComponentWithComputed>();
        var baseSignal = new Signal<int>(100);
        var computed = new Computed<int>(() => baseSignal.Value * 2, baseSignal);

        // Act
        var signalValue = component.Instance.TestUseSignal(computed);
        int value = signalValue; // Implicit conversion to T

        // Assert
        value.Should().Be(200);
    }

    [Fact]
    public void SignalValue_WithComputed_ImplicitConversionToFunc_ShouldWork()
    {
        // Arrange
        var component = RenderComponent<TestComponentWithComputed>();
        var baseSignal = new Signal<string>("hello");
        var computed = new Computed<string>(() => baseSignal.Value.ToUpper(), baseSignal);

        // Act
        var signalValue = component.Instance.TestUseSignal(computed);
        Func<string> func = signalValue; // Implicit conversion to Func<T>

        // Assert
        func().Should().Be("HELLO");
    }

    [Fact]
    public void SignalValue_WithComputed_InvokeMethod_ShouldWork()
    {
        // Arrange
        var component = RenderComponent<TestComponentWithComputed>();
        var baseSignal = new Signal<int>(5);
        var computed = new Computed<string>(() => $"Number: {baseSignal.Value}", baseSignal);

        // Act
        var signalValue = component.Instance.TestUseSignal(computed);
        var result = signalValue.Invoke();

        // Assert
        result.Should().Be("Number: 5");
    }

    [Fact]
    public void SignalValue_WithComputed_Dispose_ShouldUnsubscribe()
    {
        // Arrange
        var baseSignal = new Signal<int>(1);
        var computed = new Computed<string>(() => $"Value: {baseSignal.Value}", baseSignal);
        var component = RenderComponent<TestComponentWithComputed>();
        var signalValue = component.Instance.TestUseSignal(computed);

        var changeCount = 0;
        computed.Changed += _ => changeCount++;

        // Act
        signalValue.Dispose();
        baseSignal.Value = 2; // This should trigger computed change but not component re-render

        // Assert
        changeCount.Should().Be(1); // The computed should have changed
        // Component should not re-render after disposal
    }

    [Fact]
    public void SignalValue_WithComputed_MultipleChanges_ShouldTriggerMultipleRerenders()
    {
        // Arrange
        var baseSignal = new Signal<int>(0);
        var computed = new Computed<string>(() => baseSignal.Value % 2 == 0 ? "Even" : "Odd", baseSignal);
        Services.AddSingleton(baseSignal);
        Services.AddSingleton(computed);
        var component = RenderComponent<ComputedComponent>();

        // Act & Assert
        component.Find("p").TextContent.Should().Be("Even"); // Initial value

        baseSignal.Value = 1;
        component.Find("p").TextContent.Should().Be("Odd");

        baseSignal.Value = 2;
        component.Find("p").TextContent.Should().Be("Even");

        baseSignal.Value = 3;
        component.Find("p").TextContent.Should().Be("Odd");
    }

    [Fact]
    public void SignalValue_WithComputed_ChainedComputeds_ShouldWork()
    {
        // Arrange
        var baseSignal = new Signal<int>(10);
        var computed1 = new Computed<int>(() => baseSignal.Value * 2, baseSignal);
        var computed2 = new Computed<string>(() => $"Result: {computed1.Value}", computed1);

        var component = RenderComponent<TestComponentWithComputed>();

        // Act
        var signalValue = component.Instance.TestUseSignal(computed2);

        // Assert
        signalValue.Value.Should().Be("Result: 20");

        // Test that changes propagate through the chain
        baseSignal.Value = 5;
        signalValue.Value.Should().Be("Result: 10");
    }

    [Fact]
    public void SignalValue_WithComputed_ComplexDependency_ShouldUpdateCorrectly()
    {
        // Arrange
        var nameSignal = new Signal<string>("John");
        var ageSignal = new Signal<int>(25);
        var computed = new Computed<string>(() => $"{nameSignal.Value} ({ageSignal.Value} years old)", nameSignal, ageSignal);

        Services.AddSingleton(nameSignal);
        Services.AddSingleton(ageSignal);
        Services.AddSingleton(computed);
        var component = RenderComponent<ComputedComponent>();

        // Act & Assert
        component.Find("p").TextContent.Should().Be("John (25 years old)");

        nameSignal.Value = "Jane";
        component.Find("p").TextContent.Should().Be("Jane (25 years old)");

        ageSignal.Value = 30;
        component.Find("p").TextContent.Should().Be("Jane (30 years old)");
    }
}

// Test components for Computed tests
public class TestComponentWithComputed : ComponentBase
{
    public SignalValue<T> TestUseSignal<T>(Computed<T> computed)
    {
        return this.useSignal(computed);
    }

    public Func<T> TestUseSignalAsFunction<T>(Computed<T> computed)
    {
        return this.useSignal(computed, useAsFunction: true);
    }

    public Func<T> TestUseSignalWithFlag<T>(Computed<T> computed, bool useAsFunction)
    {
        return this.useSignal(computed, useAsFunction);
    }

    protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
        builder.AddContent(0, "Test Component for Computed");
    }
}

public class ComputedComponent : ComponentBase
{
    [Inject] public Signal<int>? BaseSignal { get; set; }
    [Inject] public Computed<string>? ComputedSignal { get; set; }

    private Func<string>? computedValue;

    protected override void OnInitialized()
    {
        if (ComputedSignal != null)
        {
            computedValue = this.useSignal(ComputedSignal);
        }
        else if (BaseSignal != null)
        {
            // Fallback for tests that only inject BaseSignal
            var computed = new Computed<string>(() => BaseSignal.Value % 2 == 0 ? "Even" : "Odd", BaseSignal);
            computedValue = this.useSignal(computed);
        }
    }

    protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "p");
        builder.AddContent(1, computedValue?.Invoke() ?? "No value");
        builder.CloseElement();
    }
}