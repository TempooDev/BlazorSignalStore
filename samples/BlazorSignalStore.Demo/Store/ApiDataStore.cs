using BlazorSignalStore.Core;

namespace BlazorSignalStore.Demo.Store
{
    /// <summary>
    /// Store that demonstrates API call optimization and state management.
    /// Shows how to avoid redundant API calls and simplify async data handling.
    /// Focuses on efficient data fetching, not heavy caching.
    /// </summary>
    public class ApiDataStore : StoreBase
    {
        // Loading states
        public Signal<bool> IsLoadingCategories { get; } = new(false);
        public Signal<bool> IsLoadingUserPreferences { get; } = new(false);
        public Signal<bool> IsLoadingFilters { get; } = new(false);

        // Data signals
        public Signal<List<Category>?> Categories { get; } = new(null);
        public Signal<UserPreferences?> UserPreferences { get; } = new(null);
        public Signal<List<Filter>?> AvailableFilters { get; } = new(null);

        // Error states
        public Signal<string?> CategoriesError { get; } = new(null);
        public Signal<string?> UserPreferencesError { get; } = new(null);
        public Signal<string?> FiltersError { get; } = new(null);

        // Activity logs for UI display
        public Signal<List<string>> ActivityLogs { get; } = new(new List<string>());
        public Signal<string?> LastRefreshedData { get; } = new(null);

        // Computed states
        public Computed<bool> HasCategories { get; }
        public Computed<bool> HasUserPreferences { get; }
        public Computed<bool> HasFilters { get; }
        public Computed<bool> IsAnyLoading { get; }
        public Computed<bool> HasAnyError { get; }
        public Computed<List<Category>> FavoriteCategories { get; }

        // Cache timestamps to control when to refetch
        private DateTime? _categoriesLastFetch;
        private DateTime? _userPreferencesLastFetch;
        private DateTime? _filtersLastFetch;

        // Data freshness tracking (10 seconds for demo purposes - shows automatic refresh)
        private readonly TimeSpan _dataFreshnessPeriod = TimeSpan.FromSeconds(10);

        public ApiDataStore()
        {
            HasCategories = new Computed<bool>(() => Categories.Value?.Any() == true, Categories);
            HasUserPreferences = new Computed<bool>(() => UserPreferences.Value != null, UserPreferences);
            HasFilters = new Computed<bool>(() => AvailableFilters.Value?.Any() == true, AvailableFilters);

            IsAnyLoading = new Computed<bool>(() =>
                IsLoadingCategories.Value || IsLoadingUserPreferences.Value || IsLoadingFilters.Value,
                IsLoadingCategories, IsLoadingUserPreferences, IsLoadingFilters);

            HasAnyError = new Computed<bool>(() =>
                !string.IsNullOrEmpty(CategoriesError.Value) ||
                !string.IsNullOrEmpty(UserPreferencesError.Value) ||
                !string.IsNullOrEmpty(FiltersError.Value),
                CategoriesError, UserPreferencesError, FiltersError);

            FavoriteCategories = new Computed<List<Category>>(() =>
            {
                if (Categories.Value == null || UserPreferences.Value == null)
                    return new List<Category>();

                return Categories.Value
                    .Where(c => UserPreferences.Value.FavoriteCategoryIds.Contains(c.Id))
                    .ToList();
            }, Categories, UserPreferences);
        }

        /// <summary>
        /// Fetches categories from API. Uses cache if data is fresh.
        /// </summary>
        public async Task LoadCategoriesAsync(bool forceRefresh = false)
        {
            // Check cache first
            if (!forceRefresh && _categoriesLastFetch.HasValue &&
                DateTime.Now - _categoriesLastFetch.Value < _dataFreshnessPeriod &&
                Categories.Value != null)
            {
                AddActivityLog("🟢 Categories: Using existing data (still fresh)");
                return;
            }

            if (IsLoadingCategories.Value)
            {
                AddActivityLog("🟡 Categories: Already loading, skipping duplicate request");
                return;
            }

            IsLoadingCategories.Value = true;
            CategoriesError.Value = null;
            AddActivityLog("🔄 Categories: Making API call...");

            try
            {
                Console.WriteLine("🔄 Categories: Fetching from API...");

                // Simulate API call
                await Task.Delay(1500);

                // Simulate occasional error
                if (Random.Shared.Next(1, 10) == 1)
                {
                    throw new HttpRequestException("Categories API temporarily unavailable");
                }

                var categories = new List<Category>
                {
                    new(1, "Electronics", "🔌", true),
                    new(2, "Books", "📚", true),
                    new(3, "Clothing", "👕", true),
                    new(4, "Home & Garden", "🏠", true),
                    new(5, "Sports", "⚽", false),
                    new(6, "Music", "🎵", true),
                    new(7, "Movies", "🎬", false),
                    new(8, "Games", "🎮", true)
                };

                Categories.Value = categories;
                _categoriesLastFetch = DateTime.Now;
                LastRefreshedData.Value = "Categories";

                AddActivityLog($"✅ Categories: Loaded {categories.Count} categories");
            }
            catch (Exception ex)
            {
                CategoriesError.Value = ex.Message;
                AddActivityLog($"❌ Categories: Error - {ex.Message}");
            }
            finally
            {
                IsLoadingCategories.Value = false;
            }
        }

        /// <summary>
        /// Fetches user preferences from API. Uses cache if data is fresh.
        /// </summary>
        public async Task LoadUserPreferencesAsync(bool forceRefresh = false)
        {
            if (!forceRefresh && _userPreferencesLastFetch.HasValue &&
                DateTime.Now - _userPreferencesLastFetch.Value < _dataFreshnessPeriod &&
                UserPreferences.Value != null)
            {
                AddActivityLog("🟢 UserPreferences: Using existing data (still fresh)");
                return;
            }

            if (IsLoadingUserPreferences.Value)
            {
                AddActivityLog("🟡 UserPreferences: Already loading, skipping duplicate request");
                return;
            }

            IsLoadingUserPreferences.Value = true;
            UserPreferencesError.Value = null;
            AddActivityLog("🔄 UserPreferences: Making API call...");

            try
            {
                Console.WriteLine("🔄 UserPreferences: Fetching from API...");

                await Task.Delay(1000);

                if (Random.Shared.Next(1, 15) == 1)
                {
                    throw new HttpRequestException("User preferences API error");
                }

                var preferences = new UserPreferences
                {
                    UserId = "user123",
                    Theme = "dark",
                    Language = "en",
                    Currency = "USD",
                    FavoriteCategoryIds = new List<int> { 1, 3, 6, 8 },
                    NotificationSettings = new NotificationSettings
                    {
                        EmailEnabled = true,
                        PushEnabled = false,
                        SmsEnabled = true
                    }
                };

                UserPreferences.Value = preferences;
                _userPreferencesLastFetch = DateTime.Now;
                LastRefreshedData.Value = "UserPreferences";

                AddActivityLog("✅ UserPreferences: Loaded user preferences");
            }
            catch (Exception ex)
            {
                UserPreferencesError.Value = ex.Message;
                AddActivityLog($"❌ UserPreferences: Error - {ex.Message}");
            }
            finally
            {
                IsLoadingUserPreferences.Value = false;
            }
        }

        /// <summary>
        /// Fetches available filters from API. Uses cache if data is fresh.
        /// </summary>
        public async Task LoadFiltersAsync(bool forceRefresh = false)
        {
            if (!forceRefresh && _filtersLastFetch.HasValue &&
                DateTime.Now - _filtersLastFetch.Value < _dataFreshnessPeriod &&
                AvailableFilters.Value != null)
            {
                AddActivityLog("🟢 Filters: Using existing data (still fresh)");
                return;
            }

            if (IsLoadingFilters.Value)
            {
                AddActivityLog("🟡 Filters: Already loading, skipping duplicate request");
                return;
            }

            IsLoadingFilters.Value = true;
            FiltersError.Value = null;
            AddActivityLog("🔄 Filters: Making API call...");

            try
            {
                Console.WriteLine("🔄 Filters: Fetching from API...");

                await Task.Delay(800);

                if (Random.Shared.Next(1, 20) == 1)
                {
                    throw new HttpRequestException("Filters API timeout");
                }

                var filters = new List<Filter>
                {
                    new("price", "Price Range", FilterType.Range, new List<string> { "0-50", "50-100", "100-500", "500+" }),
                    new("brand", "Brand", FilterType.Multiselect, new List<string> { "Apple", "Samsung", "Sony", "Nike", "Adidas" }),
                    new("rating", "Rating", FilterType.Range, new List<string> { "1+", "2+", "3+", "4+", "5" }),
                    new("availability", "Availability", FilterType.Toggle, new List<string> { "In Stock", "On Sale" }),
                    new("shipping", "Shipping", FilterType.Multiselect, new List<string> { "Free Shipping", "Fast Delivery", "Express" })
                };

                AvailableFilters.Value = filters;
                _filtersLastFetch = DateTime.Now;
                LastRefreshedData.Value = "Filters";

                AddActivityLog($"✅ Filters: Loaded {filters.Count} filter types");
            }
            catch (Exception ex)
            {
                FiltersError.Value = ex.Message;
                AddActivityLog($"❌ Filters: Error - {ex.Message}");
            }
            finally
            {
                IsLoadingFilters.Value = false;
            }
        }

        /// <summary>
        /// Loads all API data. Multiple calls are safe due to caching and loading state checks.
        /// </summary>
        public async Task LoadAllDataAsync(bool forceRefresh = false)
        {
            var tasks = new[]
            {
                LoadCategoriesAsync(forceRefresh),
                LoadUserPreferencesAsync(forceRefresh),
                LoadFiltersAsync(forceRefresh)
            };

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Clears all cached data and resets states.
        /// </summary>
        public void ClearCache()
        {
            Categories.Value = null;
            UserPreferences.Value = null;
            AvailableFilters.Value = null;

            CategoriesError.Value = null;
            UserPreferencesError.Value = null;
            FiltersError.Value = null;

            _categoriesLastFetch = null;
            _userPreferencesLastFetch = null;
            _filtersLastFetch = null;

            AddActivityLog("🗑️ Cache cleared - All data reset");
            LastRefreshedData.Value = null;
        }

        /// <summary>
        /// Adds an activity log entry for UI display.
        /// </summary>
        private void AddActivityLog(string message)
        {
            var logs = ActivityLogs.Value.ToList();
            logs.Insert(0, $"[{DateTime.Now:HH:mm:ss}] {message}");

            // Keep only last 10 entries
            if (logs.Count > 10)
                logs = logs.Take(10).ToList();

            ActivityLogs.Value = logs;
        }

        /// <summary>
        /// Auto-refresh the next data source in rotation.
        /// </summary>
        public async Task AutoRefreshNextDataSource()
        {
            // Determine which data to refresh based on last refreshed
            switch (LastRefreshedData.Value)
            {
                case null:
                case "Filters":
                    await LoadCategoriesAsync(forceRefresh: true);
                    break;
                case "Categories":
                    await LoadUserPreferencesAsync(forceRefresh: true);
                    break;
                case "UserPreferences":
                    await LoadFiltersAsync(forceRefresh: true);
                    break;
            }
        }
    }

    // Data models
    public record Category(int Id, string Name, string Icon, bool IsActive);

    public record UserPreferences
    {
        public string UserId { get; init; } = string.Empty;
        public string Theme { get; init; } = "light";
        public string Language { get; init; } = "en";
        public string Currency { get; init; } = "USD";
        public List<int> FavoriteCategoryIds { get; init; } = new();
        public NotificationSettings NotificationSettings { get; init; } = new();
    }

    public record NotificationSettings
    {
        public bool EmailEnabled { get; init; }
        public bool PushEnabled { get; init; }
        public bool SmsEnabled { get; init; }
    }

    public record Filter(string Key, string DisplayName, FilterType Type, List<string> Options);

    public enum FilterType
    {
        Toggle,
        Multiselect,
        Range
    }
}