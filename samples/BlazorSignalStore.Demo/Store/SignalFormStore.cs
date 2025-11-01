using BlazorSignalStore.Core;

namespace BlazorSignalStore.Demo.Store
{
    /// <summary>
    /// Store using BlazorSignalStore for comparison with Container State.
    /// Demonstrates granular reactivity and reduced re-renders.
    /// </summary>
    public class SignalFormStore : StoreBase
    {
        // Individual signals for form fields
        public Signal<string> FirstName { get; } = new("");
        public Signal<string> LastName { get; } = new("");
        public Signal<string> Email { get; } = new("");
        public Signal<string> Street { get; } = new("");
        public Signal<string> City { get; } = new("");
        public Signal<string> PostalCode { get; } = new("");
        public Signal<string> Country { get; } = new("");
        public Signal<bool> EmailNotifications { get; } = new(false);
        public Signal<bool> SmsNotifications { get; } = new(false);
        public Signal<string> Theme { get; } = new("Light");
        
        // State signals
        public Signal<bool> IsSubmitting { get; } = new(false);
        public Signal<SubmissionResult?> SubmissionResult { get; } = new(null);

        // Computed properties that update automatically
        public Computed<bool> IsPersonalInfoValid { get; }
        public Computed<bool> IsAddressValid { get; }
        public Computed<bool> IsPreferencesValid { get; }
        public Computed<bool> IsFormValid { get; }
        public Computed<bool> CanSubmit { get; }
        public Computed<double> FormCompletionPercentage { get; }
        public Computed<List<string>> ValidationErrors { get; }

        public SignalFormStore()
        {
            // Personal info validation
            IsPersonalInfoValid = new Computed<bool>(() =>
                !string.IsNullOrWhiteSpace(FirstName.Value) &&
                !string.IsNullOrWhiteSpace(LastName.Value) &&
                !string.IsNullOrWhiteSpace(Email.Value) &&
                Email.Value.Contains("@"),
                FirstName, LastName, Email);

            // Address validation
            IsAddressValid = new Computed<bool>(() =>
                !string.IsNullOrWhiteSpace(Street.Value) &&
                !string.IsNullOrWhiteSpace(City.Value) &&
                !string.IsNullOrWhiteSpace(PostalCode.Value) &&
                !string.IsNullOrWhiteSpace(Country.Value),
                Street, City, PostalCode, Country);

            // Preferences validation
            IsPreferencesValid = new Computed<bool>(() =>
                !string.IsNullOrWhiteSpace(Theme.Value),
                Theme);

            // Overall form validation
            IsFormValid = new Computed<bool>(() =>
                IsPersonalInfoValid.Value && IsAddressValid.Value && IsPreferencesValid.Value,
                IsPersonalInfoValid, IsAddressValid, IsPreferencesValid);

            // Can submit
            CanSubmit = new Computed<bool>(() =>
                IsFormValid.Value && !IsSubmitting.Value,
                IsFormValid, IsSubmitting);

            // Form completion percentage
            FormCompletionPercentage = new Computed<double>(() =>
            {
                var totalFields = 9;
                var completedFields = 0;

                if (!string.IsNullOrWhiteSpace(FirstName.Value)) completedFields++;
                if (!string.IsNullOrWhiteSpace(LastName.Value)) completedFields++;
                if (!string.IsNullOrWhiteSpace(Email.Value)) completedFields++;
                if (!string.IsNullOrWhiteSpace(Street.Value)) completedFields++;
                if (!string.IsNullOrWhiteSpace(City.Value)) completedFields++;
                if (!string.IsNullOrWhiteSpace(PostalCode.Value)) completedFields++;
                if (!string.IsNullOrWhiteSpace(Country.Value)) completedFields++;
                if (!string.IsNullOrWhiteSpace(Theme.Value)) completedFields++;
                if (EmailNotifications.Value || SmsNotifications.Value) completedFields++;

                return (double)completedFields / totalFields * 100;
            }, FirstName, LastName, Email, Street, City, PostalCode, Country, Theme, EmailNotifications, SmsNotifications);

            // Validation errors
            ValidationErrors = new Computed<List<string>>(() =>
            {
                var errors = new List<string>();

                // Personal info errors
                if (string.IsNullOrWhiteSpace(FirstName.Value))
                    errors.Add("First name is required");
                if (string.IsNullOrWhiteSpace(LastName.Value))
                    errors.Add("Last name is required");
                if (string.IsNullOrWhiteSpace(Email.Value))
                    errors.Add("Email is required");
                else if (!Email.Value.Contains("@"))
                    errors.Add("Email format is invalid");

                // Address errors
                if (string.IsNullOrWhiteSpace(Street.Value))
                    errors.Add("Street address is required");
                if (string.IsNullOrWhiteSpace(City.Value))
                    errors.Add("City is required");
                if (string.IsNullOrWhiteSpace(PostalCode.Value))
                    errors.Add("Postal code is required");
                if (string.IsNullOrWhiteSpace(Country.Value))
                    errors.Add("Country is required");

                // Preferences errors
                if (string.IsNullOrWhiteSpace(Theme.Value))
                    errors.Add("Theme selection is required");

                return errors;
            }, FirstName, LastName, Email, Street, City, PostalCode, Country, Theme);
        }

        public async Task SubmitFormAsync()
        {
            if (!CanSubmit.Value) return;

            IsSubmitting.Value = true;

            try
            {
                // Simulate API call
                await Task.Delay(2000);

                // Simulate random success/failure
                var success = Random.Shared.NextDouble() > 0.3;
                
                IsSubmitting.Value = false;
                SubmissionResult.Value = new SubmissionResult(
                    success, 
                    success ? "Form submitted successfully!" : "Submission failed. Please try again.");
            }
            catch (Exception ex)
            {
                IsSubmitting.Value = false;
                SubmissionResult.Value = new SubmissionResult(false, $"Error: {ex.Message}");
            }
        }

        public void ResetForm()
        {
            FirstName.Value = "";
            LastName.Value = "";
            Email.Value = "";
            Street.Value = "";
            City.Value = "";
            PostalCode.Value = "";
            Country.Value = "";
            EmailNotifications.Value = false;
            SmsNotifications.Value = false;
            Theme.Value = "Light";
            IsSubmitting.Value = false;
            SubmissionResult.Value = null;
        }

        public void ClearSubmissionResult()
        {
            SubmissionResult.Value = null;
        }
    }
}