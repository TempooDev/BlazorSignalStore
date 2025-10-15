using BlazorSignalStore.Core;

namespace BlazorSignalStore.Demo.Store
{
    /// <summary>
    /// Shopping cart store demonstrating shared state across multiple components.
    /// </summary>
    public class ShoppingCartStore : StoreBase
    {
        /// <summary>
        /// Available products in the store.
        /// </summary>
        public Signal<List<Product>> Products { get; } = new(new List<Product>
        {
            new(1, "Laptop Pro", 1299.99m, "High-performance laptop for developers"),
            new(2, "Wireless Mouse", 39.99m, "Ergonomic wireless mouse"),
            new(3, "Mechanical Keyboard", 129.99m, "RGB mechanical keyboard"),
            new(4, "Monitor 4K", 399.99m, "27-inch 4K display"),
            new(5, "USB-C Hub", 79.99m, "Multi-port USB-C hub"),
            new(6, "Webcam HD", 89.99m, "1080p webcam with auto-focus")
        });

        /// <summary>
        /// Items currently in the shopping cart.
        /// </summary>
        public Signal<List<CartItem>> CartItems { get; } = new(new List<CartItem>());

        /// <summary>
        /// Computed total number of items in the cart.
        /// </summary>
        public Computed<int> ItemCount { get; }

        /// <summary>
        /// Computed total price of all items in the cart.
        /// </summary>
        public Computed<decimal> TotalPrice { get; }

        /// <summary>
        /// Computed formatted total price.
        /// </summary>
        public Computed<string> FormattedTotal { get; }

        /// <summary>
        /// Computed flag indicating if the cart is empty.
        /// </summary>
        public Computed<bool> IsEmpty { get; }

        /// <summary>
        /// Initializes the shopping cart store with computed properties.
        /// </summary>
        public ShoppingCartStore()
        {
            ItemCount = new Computed<int>(() =>
                CartItems.Value.Sum(item => item.Quantity), CartItems);

            TotalPrice = new Computed<decimal>(() =>
                CartItems.Value.Sum(item => item.Product.Price * item.Quantity), CartItems);

            FormattedTotal = new Computed<string>(() =>
                $"${TotalPrice.Value:F2}", TotalPrice);

            IsEmpty = new Computed<bool>(() =>
                CartItems.Value.Count == 0, CartItems);
        }

        /// <summary>
        /// Adds a product to the cart or increases its quantity if already present.
        /// </summary>
        /// <param name="product">The product to add.</param>
        public void AddToCart(Product product)
        {
            var currentItems = new List<CartItem>(CartItems.Value);
            var existingItem = currentItems.FirstOrDefault(item => item.Product.Id == product.Id);

            if (existingItem != null)
            {
                // Update quantity of existing item
                var index = currentItems.IndexOf(existingItem);
                currentItems[index] = existingItem with { Quantity = existingItem.Quantity + 1 };
            }
            else
            {
                // Add new item
                currentItems.Add(new CartItem(product, 1));
            }

            CartItems.Value = currentItems;
        }

        /// <summary>
        /// Removes a product from the cart or decreases its quantity.
        /// </summary>
        /// <param name="productId">The ID of the product to remove.</param>
        public void RemoveFromCart(int productId)
        {
            var currentItems = new List<CartItem>(CartItems.Value);
            var existingItem = currentItems.FirstOrDefault(item => item.Product.Id == productId);

            if (existingItem != null)
            {
                if (existingItem.Quantity > 1)
                {
                    // Decrease quantity
                    var index = currentItems.IndexOf(existingItem);
                    currentItems[index] = existingItem with { Quantity = existingItem.Quantity - 1 };
                }
                else
                {
                    // Remove item completely
                    currentItems.Remove(existingItem);
                }

                CartItems.Value = currentItems;
            }
        }

        /// <summary>
        /// Clears all items from the cart.
        /// </summary>
        public void ClearCart()
        {
            // Create a completely new list to ensure signal change detection
            var emptyList = new List<CartItem>();
            CartItems.Value = emptyList;
        }

        /// <summary>
        /// Gets the quantity of a specific product in the cart.
        /// </summary>
        /// <param name="productId">The product ID to check.</param>
        /// <returns>The quantity of the product in the cart.</returns>
        public int GetProductQuantity(int productId)
        {
            return CartItems.Value.FirstOrDefault(item => item.Product.Id == productId)?.Quantity ?? 0;
        }
    }

    /// <summary>
    /// Represents a product in the store.
    /// </summary>
    /// <param name="Id">Unique identifier for the product.</param>
    /// <param name="Name">Display name of the product.</param>
    /// <param name="Price">Price of the product.</param>
    /// <param name="Description">Description of the product.</param>
    public record Product(int Id, string Name, decimal Price, string Description);

    /// <summary>
    /// Represents an item in the shopping cart.
    /// </summary>
    /// <param name="Product">The product.</param>
    /// <param name="Quantity">The quantity of this product in the cart.</param>
    public record CartItem(Product Product, int Quantity);
}