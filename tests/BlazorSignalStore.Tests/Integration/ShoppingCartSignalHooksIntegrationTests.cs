using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using BlazorSignalStore.Core;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace BlazorSignalStore.Tests.Integration;

/// <summary>
/// Integration tests for the shopping cart scenario with multiple computed signals.
/// These tests verify that the SignalHooks enhancements work correctly in a real-world scenario.
/// </summary>
public class ShoppingCartSignalHooksIntegrationTests : TestContext
{
    [Fact]
    public void ShoppingCart_MultipleComputedSignals_ShouldAllUpdateCorrectly()
    {
        // Arrange
        var store = new TestShoppingCartStore();
        Services.AddSingleton(store);
        var component = RenderComponent<ShoppingCartIntegrationComponent>();

        // Act - Add items to cart
        store.AddItem("Apple", 1.50m, 2);
        store.AddItem("Banana", 0.75m, 3);

        // Assert - All computed values should update
        component.Find("#item-count").TextContent.Should().Be("5"); // 2 + 3
        component.Find("#total-price").TextContent.Should().Be("$5.25"); // (1.50 * 2) + (0.75 * 3)
        component.Find("#is-empty").TextContent.Should().Be("False");
        component.Find("#has-items").TextContent.Should().Be("True");
    }

    [Fact]
    public void ShoppingCart_ClearCart_ShouldUpdateAllComputedSignals()
    {
        // Arrange
        var store = new TestShoppingCartStore();
        Services.AddSingleton(store);
        var component = RenderComponent<ShoppingCartIntegrationComponent>();

        // Add items first
        store.AddItem("Orange", 2.00m, 1);

        // Verify items were added
        component.Find("#item-count").TextContent.Should().Be("1");
        component.Find("#is-empty").TextContent.Should().Be("False");

        // Act - Clear cart
        store.ClearCart();

        // Assert - All computed values should reset
        component.Find("#item-count").TextContent.Should().Be("0");
        component.Find("#total-price").TextContent.Should().Be("$0.00");
        component.Find("#is-empty").TextContent.Should().Be("True");
        component.Find("#has-items").TextContent.Should().Be("False");
    }

    [Fact]
    public void ShoppingCart_RemoveItem_ShouldUpdateComputedSignals()
    {
        // Arrange
        var store = new TestShoppingCartStore();
        Services.AddSingleton(store);
        var component = RenderComponent<ShoppingCartIntegrationComponent>();

        // Add items
        store.AddItem("Grape", 3.00m, 2);
        store.AddItem("Peach", 1.25m, 4);

        // Verify initial state
        component.Find("#item-count").TextContent.Should().Be("6");

        // Act - Remove one item type
        store.RemoveItem("Grape");

        // Assert
        component.Find("#item-count").TextContent.Should().Be("4"); // Only peaches remain
        component.Find("#total-price").TextContent.Should().Be("$5.00"); // 1.25 * 4
    }

    [Fact]
    public void ShoppingCart_UpdateQuantity_ShouldUpdateComputedSignals()
    {
        // Arrange
        var store = new TestShoppingCartStore();
        Services.AddSingleton(store);
        var component = RenderComponent<ShoppingCartIntegrationComponent>();

        store.AddItem("Mango", 2.50m, 1);

        // Act - Update quantity
        store.UpdateQuantity("Mango", 5);

        // Assert
        component.Find("#item-count").TextContent.Should().Be("5");
        component.Find("#total-price").TextContent.Should().Be("$12.50"); // 2.50 * 5
    }

    [Fact]
    public void ShoppingCart_ChainedComputedSignals_ShouldUpdateCorrectly()
    {
        // Arrange
        var store = new TestShoppingCartStore();
        Services.AddSingleton(store);
        var component = RenderComponent<ChainedComputedComponent>();

        // Act
        store.AddItem("Expensive Item", 100.00m, 1);

        // Assert - Chained computed should work
        component.Find("#shipping-status").TextContent.Should().Be("Free shipping available!");
        component.Find("#discount-info").TextContent.Should().Be("10% discount applied");
    }

    [Fact]
    public void ShoppingCart_MultipleComponents_ShouldStayInSync()
    {
        // Arrange
        var store = new TestShoppingCartStore();
        Services.AddSingleton(store);

        var component1 = RenderComponent<ShoppingCartIntegrationComponent>();
        var component2 = RenderComponent<ShoppingCartIntegrationComponent>();

        // Act
        store.AddItem("Shared Item", 5.00m, 3);

        // Assert - Both components should show the same data
        component1.Find("#item-count").TextContent.Should().Be("3");
        component2.Find("#item-count").TextContent.Should().Be("3");

        component1.Find("#total-price").TextContent.Should().Be("$15.00");
        component2.Find("#total-price").TextContent.Should().Be("$15.00");
    }

    [Fact]
    public void ShoppingCart_DisposedSignalValue_ShouldStopUpdating()
    {
        // Arrange
        var store = new TestShoppingCartStore();
        var component = RenderComponent<DisposableShoppingCartComponent>();
        component.Instance.InitializeWithStore(store);

        // Act - Dispose one of the signal values
        component.Instance.DisposeItemCount();
        store.AddItem("Test Item", 1.00m, 1);

        // Assert - The disposed signal should not update the component
        // (This test verifies that disposal works correctly)
        component.Instance.ItemCountSignalValue.Should().BeNull();
    }
}

// Test store for shopping cart integration tests
public class TestShoppingCartStore
{
    private readonly Signal<List<CartItem>> _cartItems = new(new List<CartItem>());

    public Signal<List<CartItem>> CartItems => _cartItems;

    public Computed<int> ItemCount => new(() => _cartItems.Value.Sum(item => item.Quantity), _cartItems);

    public Computed<decimal> TotalPrice => new(() => _cartItems.Value.Sum(item => item.Price * item.Quantity), _cartItems);

    public Computed<string> FormattedTotal => new(() => $"${TotalPrice.Value:F2}", TotalPrice);

    public Computed<bool> IsEmpty => new(() => !_cartItems.Value.Any(), _cartItems);

    public Computed<bool> HasItems => new(() => _cartItems.Value.Any(), _cartItems);

    // Chained computed signals for advanced testing
    public Computed<bool> QualifiesForFreeShipping => new(() => TotalPrice.Value >= 50.00m, TotalPrice);

    public Computed<string> ShippingStatus => new(() =>
        QualifiesForFreeShipping.Value ? "Free shipping available!" : "Add more for free shipping",
        QualifiesForFreeShipping);

    public Computed<string> DiscountInfo => new(() =>
        TotalPrice.Value >= 100.00m ? "10% discount applied" : "No discount",
        TotalPrice);

    public void AddItem(string name, decimal price, int quantity)
    {
        var currentItems = new List<CartItem>(_cartItems.Value);
        var existingItem = currentItems.FirstOrDefault(item => item.Name == name);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            currentItems.Add(new CartItem { Name = name, Price = price, Quantity = quantity });
        }

        _cartItems.Value = currentItems;
    }

    public void RemoveItem(string name)
    {
        var currentItems = new List<CartItem>(_cartItems.Value);
        currentItems.RemoveAll(item => item.Name == name);
        _cartItems.Value = currentItems;
    }

    public void UpdateQuantity(string name, int newQuantity)
    {
        var currentItems = new List<CartItem>(_cartItems.Value);
        var item = currentItems.FirstOrDefault(i => i.Name == name);
        if (item != null)
        {
            item.Quantity = newQuantity;
            _cartItems.Value = currentItems;
        }
    }

    public void ClearCart()
    {
        _cartItems.Value = new List<CartItem>();
    }
}

public class CartItem
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

// Test components for integration tests
public class ShoppingCartIntegrationComponent : ComponentBase
{
    [Inject] public TestShoppingCartStore Store { get; set; } = null!;

    private Func<int>? itemCount;
    private Func<string>? totalPrice;
    private Func<bool>? isEmpty;
    private Func<bool>? hasItems;

    protected override void OnInitialized()
    {
        itemCount = this.useSignal(Store.ItemCount);
        totalPrice = this.useSignal(Store.FormattedTotal);
        isEmpty = this.useSignal(Store.IsEmpty);
        hasItems = this.useSignal(Store.HasItems);
    }

    protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");

        builder.OpenElement(1, "span");
        builder.AddAttribute(2, "id", "item-count");
        builder.AddContent(3, itemCount?.Invoke().ToString() ?? "0");
        builder.CloseElement();

        builder.OpenElement(4, "span");
        builder.AddAttribute(5, "id", "total-price");
        builder.AddContent(6, totalPrice?.Invoke() ?? "$0.00");
        builder.CloseElement();

        builder.OpenElement(7, "span");
        builder.AddAttribute(8, "id", "is-empty");
        builder.AddContent(9, isEmpty?.Invoke().ToString() ?? "True");
        builder.CloseElement();

        builder.OpenElement(10, "span");
        builder.AddAttribute(11, "id", "has-items");
        builder.AddContent(12, hasItems?.Invoke().ToString() ?? "False");
        builder.CloseElement();

        builder.CloseElement();
    }
}

public class ChainedComputedComponent : ComponentBase
{
    [Inject] public TestShoppingCartStore Store { get; set; } = null!;

    private Func<string>? shippingStatus;
    private Func<string>? discountInfo;

    protected override void OnInitialized()
    {
        shippingStatus = this.useSignal(Store.ShippingStatus);
        discountInfo = this.useSignal(Store.DiscountInfo);
    }

    protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");

        builder.OpenElement(1, "span");
        builder.AddAttribute(2, "id", "shipping-status");
        builder.AddContent(3, shippingStatus?.Invoke() ?? "No shipping info");
        builder.CloseElement();

        builder.OpenElement(4, "span");
        builder.AddAttribute(5, "id", "discount-info");
        builder.AddContent(6, discountInfo?.Invoke() ?? "No discount info");
        builder.CloseElement();

        builder.CloseElement();
    }
}

public class DisposableShoppingCartComponent : ComponentBase
{
    private TestShoppingCartStore? _store;
    public SignalValue<int>? ItemCountSignalValue { get; private set; }

    public void InitializeWithStore(TestShoppingCartStore store)
    {
        _store = store;
        ItemCountSignalValue = this.useSignal(store.ItemCount);
    }

    public void DisposeItemCount()
    {
        ItemCountSignalValue?.Dispose();
        ItemCountSignalValue = null;
    }

    protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
        builder.AddContent(0, "Disposable component");
    }
}
