using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using BlazorSignalStore.Core;

namespace BlazorSignalStore.Tests.Integration;

public class CounterStoreIntegrationTests : TestContext
{
    [Fact]
    public void CounterStore_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var store = new CounterStore();

        // Assert
        store.Count.Value.Should().Be(0);
        store.Label.Value.Should().Be("Count: 0");
    }

    [Fact]
    public void CounterStore_Increment_ShouldUpdateBothSignals()
    {
        // Arrange
        var store = new CounterStore();
        var countChanges = new List<int>();
        var labelChanges = new List<string>();

        store.Count.Changed += value => countChanges.Add(value);
        store.Label.Changed += value => labelChanges.Add(value);

        // Act
        store.Increment();
        store.Increment();

        // Assert
        store.Count.Value.Should().Be(2);
        store.Label.Value.Should().Be("Count: 2");

        countChanges.Should().Equal(1, 2);
        labelChanges.Should().Equal("Count: 1", "Count: 2");
    }

    [Fact]
    public void CounterStore_Decrement_ShouldUpdateBothSignals()
    {
        // Arrange
        var store = new CounterStore();
        store.Count.Value = 5; // Start at 5

        // Act
        store.Decrement();
        store.Decrement();

        // Assert
        store.Count.Value.Should().Be(3);
        store.Label.Value.Should().Be("Count: 3");
    }

    [Fact]
    public void CounterStore_MixedOperations_ShouldMaintainConsistency()
    {
        // Arrange
        var store = new CounterStore();

        // Act
        store.Increment(); // 1
        store.Increment(); // 2
        store.Decrement(); // 1
        store.Increment(); // 2
        store.Increment(); // 3

        // Assert
        store.Count.Value.Should().Be(3);
        store.Label.Value.Should().Be("Count: 3");
    }

    [Fact]
    public void CounterStore_InBlazorComponent_ShouldWork()
    {
        // Arrange
        Services.AddSignalStore<CounterStore>();
        var component = RenderComponent<CounterTestComponent>();

        // Act & Assert
        // Initial state
        component.Find("#count").TextContent.Should().Be("0");
        component.Find("#label").TextContent.Should().Be("Count: 0");

        // Click increment button
        component.Find("#increment").Click();
        component.Find("#count").TextContent.Should().Be("1");
        component.Find("#label").TextContent.Should().Be("Count: 1");

        // Click decrement button
        component.Find("#decrement").Click();
        component.Find("#count").TextContent.Should().Be("0");
        component.Find("#label").TextContent.Should().Be("Count: 0");
    }

    [Fact]
    public void CounterStore_Subscribe_ShouldReceiveAllChanges()
    {
        // Arrange
        var store = new CounterStore();
        var countValues = new List<int>();
        var labelValues = new List<string>();

        // Act
        using var countSubscription = store.Count.Subscribe(value => countValues.Add(value));
        using var labelSubscription = store.Label.Subscribe(value => labelValues.Add(value));

        store.Increment();
        store.Increment();
        store.Decrement();

        // Assert
        countValues.Should().Equal(0, 1, 2, 1); // Initial + changes
        labelValues.Should().Equal("Count: 0", "Count: 1", "Count: 2", "Count: 1");
    }
}

// Test component for integration testing
public class CounterTestComponent : Microsoft.AspNetCore.Components.ComponentBase
{
    [Microsoft.AspNetCore.Components.Inject]
    public CounterStore Store { get; set; } = null!;

    private Func<int>? count;
    private Func<string>? label;

    protected override void OnInitialized()
    {
        count = this.useSignal(Store.Count);
        label = this.useSignal(Store.Label);
    }

    protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");

        builder.OpenElement(1, "span");
        builder.AddAttribute(2, "id", "count");
        builder.AddContent(3, count?.Invoke().ToString() ?? "0");
        builder.CloseElement();

        builder.OpenElement(4, "span");
        builder.AddAttribute(5, "id", "label");
        builder.AddContent(6, label?.Invoke() ?? "");
        builder.CloseElement();

        builder.OpenElement(7, "button");
        builder.AddAttribute(8, "id", "increment");
        builder.AddAttribute(9, "onclick", Microsoft.AspNetCore.Components.EventCallback.Factory.Create(this, Store.Increment));
        builder.AddContent(10, "Increment");
        builder.CloseElement();

        builder.OpenElement(11, "button");
        builder.AddAttribute(12, "id", "decrement");
        builder.AddAttribute(13, "onclick", Microsoft.AspNetCore.Components.EventCallback.Factory.Create(this, Store.Decrement));
        builder.AddContent(14, "Decrement");
        builder.CloseElement();

        builder.CloseElement();
    }
}

// Copy of CounterStore for testing (to avoid circular dependencies)
public class CounterStore : StoreBase
{
    public Signal<int> Count { get; } = new(0);
    public Computed<string> Label { get; }

    public CounterStore()
    {
        Label = new Computed<string>(() => $"Count: {Count.Value}", Count);
    }

    public void Increment() => Count.Value++;
    public void Decrement() => Count.Value--;
}
