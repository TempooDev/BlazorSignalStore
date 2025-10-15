using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using BlazorSignalStore.Core;

namespace BlazorSignalStore.Tests.Core;

public class SignalHooksTests : TestContext
{
    [Fact]
    public void UseSignal_ShouldReturnSignalValue()
    {
        // Arrange
        var component = RenderComponent<TestComponentWithSignal>();
        var signal = new Signal<int>(42);

        // Act
        var signalValue = component.Instance.TestUseSignal(signal);

        // Assert
        signalValue.Should().NotBeNull();
        signalValue.Value.Should().Be(42);
    }

    [Fact]
    public void UseSignal_ShouldReturnFunction()
    {
        // Arrange
        var component = RenderComponent<TestComponentWithSignal>();
        var signal = new Signal<string>("Hello");

        // Act
        Func<string> signalFunc = component.Instance.TestUseSignal(signal);

        // Assert
        signalFunc.Should().NotBeNull();
        signalFunc().Should().Be("Hello");
    }

    [Fact]
    public void UseSignal_WithComputed_ShouldWork()
    {
        // Arrange
        var component = RenderComponent<TestComponentWithSignal>();
        var baseSignal = new Signal<int>(10);
        var computed = new Computed<string>(() => $"Value: {baseSignal.Value}", baseSignal);

        // Act
        var signalValue = component.Instance.TestUseSignal(computed);

        // Assert
        signalValue.Value.Should().Be("Value: 10");
    }

    [Fact]
    public void UseSignal_ShouldTriggerComponentRerender()
    {
        // Arrange
        var signal = new Signal<int>(1);
        Services.AddSingleton(signal);
        var component = RenderComponent<CounterComponent>();

        // Act
        signal.Value = 2;

        // Assert
        // The component should have re-rendered with the new value
        component.Find("p").TextContent.Should().Be("Count: 2");
    }

    [Fact]
    public void UseSignal_MultipleSignals_ShouldAllTriggerRerender()
    {
        // Arrange
        var countSignal = new Signal<int>(0);
        var nameSignal = new Signal<string>("Test");
        Services.AddSingleton(countSignal);
        Services.AddSingleton(nameSignal);
        var component = RenderComponent<MultiSignalComponent>();

        // Act
        countSignal.Value = 5;
        nameSignal.Value = "Updated";

        // Assert
        component.Find("#count").TextContent.Should().Be("5");
        component.Find("#name").TextContent.Should().Be("Updated");
    }

    [Fact]
    public void SignalValue_ImplicitConversionToFunc_ShouldWork()
    {
        // Arrange
        var component = RenderComponent<TestComponentWithSignal>();
        var signal = new Signal<double>(3.14);

        // Act
        var signalValue = component.Instance.TestUseSignal(signal);
        Func<double> func = signalValue; // Implicit conversion

        // Assert
        func().Should().Be(3.14);
    }

    [Fact]
    public void SignalValue_Dispose_ShouldUnsubscribe()
    {
        // Arrange
        var signal = new Signal<int>(1);
        var component = RenderComponent<TestComponentWithSignal>();
        var signalValue = component.Instance.TestUseSignal(signal);
        var changeCount = 0;

        // Subscribe to changes to verify subscription
        signal.Changed += _ => changeCount++;

        // Act
        signalValue.Dispose();
        signal.Value = 2; // This should not trigger component re-render

        // Assert
        changeCount.Should().Be(1); // Only the signal itself should have changed
        // Component should not re-render after disposal
    }
}

// Test components for the unit tests
public class TestComponentWithSignal : ComponentBase
{
    public SignalValue<T> TestUseSignal<T>(Signal<T> signal)
    {
        return this.useSignal(signal);
    }

    protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
        builder.AddContent(0, "Test Component");
    }
}

public class CounterComponent : ComponentBase
{
    [Inject] public Signal<int> CountSignal { get; set; } = null!;

    private Func<int>? count;

    protected override void OnInitialized()
    {
        count = this.useSignal(CountSignal);
    }

    protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "p");
        builder.AddContent(1, $"Count: {count?.Invoke() ?? 0}");
        builder.CloseElement();
    }
}

public class MultiSignalComponent : ComponentBase
{
    [Inject] public Signal<int> CountSignal { get; set; } = null!;
    [Inject] public Signal<string> NameSignal { get; set; } = null!;

    private Func<int>? count;
    private Func<string>? name;

    protected override void OnInitialized()
    {
        count = this.useSignal(CountSignal);
        name = this.useSignal(NameSignal);
    }

    protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");

        builder.OpenElement(1, "span");
        builder.AddAttribute(2, "id", "count");
        builder.AddContent(3, count?.Invoke().ToString() ?? "0");
        builder.CloseElement();

        builder.OpenElement(4, "span");
        builder.AddAttribute(5, "id", "name");
        builder.AddContent(6, name?.Invoke() ?? "");
        builder.CloseElement();

        builder.CloseElement();
    }
}