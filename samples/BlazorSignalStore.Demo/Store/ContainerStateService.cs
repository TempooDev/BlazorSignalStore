namespace BlazorSignalStore.Demo.Store
{
    /// <summary>
    /// Container State pattern demonstration service.
    /// This pattern encapsulates multiple related state values into a single container,
    /// as recommended by Microsoft for efficient state management in Blazor applications.
    /// 
    /// Benefits:
    /// - Reduces the number of change notifications
    /// - Groups related state together
    /// - Simplifies state updates
    /// - Improves performance by batching updates
    /// </summary>
    public class ContainerStateService
    {
        private FormContainerState _formState = new();
        
        public event Action? OnChange;

        public FormContainerState FormState => _formState;

        // Properties derived from the container state
        public bool IsFormValid => _formState.IsValid();
        public bool CanSubmit => _formState.CanSubmit();
        public List<string> ValidationErrors => _formState.GetValidationErrors();
        public double FormCompletionPercentage => _formState.GetCompletionPercentage();

        // Actions that modify the container state
        public void UpdatePersonalInfo(string firstName, string lastName, string email)
        {
            _formState = _formState with
            {
                PersonalInfo = _formState.PersonalInfo with
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email
                }
            };
            NotifyStateChanged();
        }

        public void UpdateAddress(string street, string city, string postalCode, string country)
        {
            _formState = _formState with
            {
                Address = _formState.Address with
                {
                    Street = street,
                    City = city,
                    PostalCode = postalCode,
                    Country = country
                }
            };
            NotifyStateChanged();
        }

        public void UpdatePreferences(bool emailNotifications, bool smsNotifications, string theme)
        {
            _formState = _formState with
            {
                Preferences = _formState.Preferences with
                {
                    EmailNotifications = emailNotifications,
                    SmsNotifications = smsNotifications,
                    Theme = theme
                }
            };
            NotifyStateChanged();
        }

        public void SetSubmitting(bool isSubmitting)
        {
            _formState = _formState with { IsSubmitting = isSubmitting };
            NotifyStateChanged();
        }

        public async Task SubmitFormAsync()
        {
            if (!CanSubmit) return;

            SetSubmitting(true);

            try
            {
                // Simulate API call
                await Task.Delay(2000);

                // Simulate random success/failure
                var success = Random.Shared.NextDouble() > 0.3;
                
                _formState = _formState with
                {
                    IsSubmitting = false,
                    SubmissionResult = new SubmissionResult(
                        success, 
                        success ? "Form submitted successfully!" : "Submission failed. Please try again.")
                };
            }
            catch (Exception ex)
            {
                _formState = _formState with
                {
                    IsSubmitting = false,
                    SubmissionResult = new SubmissionResult(false, $"Error: {ex.Message}")
                };
            }

            NotifyStateChanged();
        }

        public void ResetForm()
        {
            _formState = new FormContainerState();
            NotifyStateChanged();
        }

        public void ClearSubmissionResult()
        {
            _formState = _formState with { SubmissionResult = null };
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }

    /// <summary>
    /// Container State that holds all related form data.
    /// Using records for immutability as recommended by Microsoft.
    /// This approach reduces the number of change notifications and groups related state.
    /// </summary>
    public record FormContainerState
    {
        public PersonalInfo PersonalInfo { get; init; } = new();
        public Address Address { get; init; } = new();
        public UserPreferencesData Preferences { get; init; } = new();
        public bool IsSubmitting { get; init; } = false;
        public SubmissionResult? SubmissionResult { get; init; } = null;

        // Validation logic within the container
        public bool IsValid()
        {
            return PersonalInfo.IsValid() && Address.IsValid() && Preferences.IsValid();
        }

        public bool CanSubmit()
        {
            return IsValid() && !IsSubmitting;
        }

        public List<string> GetValidationErrors()
        {
            var errors = new List<string>();
            errors.AddRange(PersonalInfo.GetValidationErrors());
            errors.AddRange(Address.GetValidationErrors());
            errors.AddRange(Preferences.GetValidationErrors());
            return errors;
        }

        public double GetCompletionPercentage()
        {
            var totalFields = 9; // Total required fields
            var completedFields = 0;

            if (!string.IsNullOrWhiteSpace(PersonalInfo.FirstName)) completedFields++;
            if (!string.IsNullOrWhiteSpace(PersonalInfo.LastName)) completedFields++;
            if (!string.IsNullOrWhiteSpace(PersonalInfo.Email)) completedFields++;
            if (!string.IsNullOrWhiteSpace(Address.Street)) completedFields++;
            if (!string.IsNullOrWhiteSpace(Address.City)) completedFields++;
            if (!string.IsNullOrWhiteSpace(Address.PostalCode)) completedFields++;
            if (!string.IsNullOrWhiteSpace(Address.Country)) completedFields++;
            if (!string.IsNullOrWhiteSpace(Preferences.Theme)) completedFields++;
            if (Preferences.EmailNotifications || Preferences.SmsNotifications) completedFields++;

            return (double)completedFields / totalFields * 100;
        }
    }

    public record PersonalInfo
    {
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   Email.Contains("@");
        }

        public List<string> GetValidationErrors()
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(FirstName))
                errors.Add("First name is required");
            if (string.IsNullOrWhiteSpace(LastName))
                errors.Add("Last name is required");
            if (string.IsNullOrWhiteSpace(Email))
                errors.Add("Email is required");
            else if (!Email.Contains("@"))
                errors.Add("Email format is invalid");
            return errors;
        }
    }

    public record Address
    {
        public string Street { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string PostalCode { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Street) &&
                   !string.IsNullOrWhiteSpace(City) &&
                   !string.IsNullOrWhiteSpace(PostalCode) &&
                   !string.IsNullOrWhiteSpace(Country);
        }

        public List<string> GetValidationErrors()
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(Street))
                errors.Add("Street address is required");
            if (string.IsNullOrWhiteSpace(City))
                errors.Add("City is required");
            if (string.IsNullOrWhiteSpace(PostalCode))
                errors.Add("Postal code is required");
            if (string.IsNullOrWhiteSpace(Country))
                errors.Add("Country is required");
            return errors;
        }
    }

    public record UserPreferencesData
    {
        public bool EmailNotifications { get; init; } = false;
        public bool SmsNotifications { get; init; } = false;
        public string Theme { get; init; } = "Light";

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Theme);
        }

        public List<string> GetValidationErrors()
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(Theme))
                errors.Add("Theme selection is required");
            return errors;
        }
    }

    public record SubmissionResult(bool Success, string? Message);
}