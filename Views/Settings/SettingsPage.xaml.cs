using doanC_.ViewModels;
using doanC_;
using doanC_.Services.Localization;

namespace doanC_.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        this.BindingContext = new SettingsViewModel();
        LoadSettings();
    }

    private void LoadSettings()
    {
        var savedLanguage = Preferences.Get("AppLanguage", "vi");
        var savedVoice = Preferences.Get("SelectedVoice", "Giọng nữ miền Nam");
        var savedRadius = Preferences.Get("GeoFenceRadius", "50 mét");
        var backgroundTracking = Preferences.Get("BackgroundTracking", true);
        var offlinePackage = Preferences.Get("OfflinePackage", "Phố Lê Thánh Tôn · 24MB");

        // Update language label based on saved language code
        UpdateLanguageLabel(savedLanguage);

        if (VoiceLabel != null) VoiceLabel.Text = savedVoice;
        if (RadiusLabel != null) RadiusLabel.Text = savedRadius;
        if (BackgroundTrackingSwitch != null) BackgroundTrackingSwitch.IsToggled = backgroundTracking;
        if (OfflinePackageLabel != null) OfflinePackageLabel.Text = offlinePackage;
    }

    private void UpdateLanguageLabel(string languageCode)
    {
        var languageNames = new Dictionary<string, string>
        {
            { "vi", "🇻🇳 Tiếng Việt" },
            { "en", "🇺🇸 English" },
            { "zh", "🇨🇳 中文 (Chinese)" },
            { "fr", "🇫🇷 Français (French)" },
            { "es", "🇪🇸 Español (Spanish)" },
            { "ja", "🇯🇵 日本語 (Japanese)" },
            { "ko", "🇰🇷 한국어 (Korean)" }
        };

        if (LanguageLabel != null && languageNames.TryGetValue(languageCode, out var displayName))
        {
            LanguageLabel.Text = displayName;
        }
    }

    private async void OnLanguageClicked(object sender, EventArgs e)
    {
        string[] languages =
        {
            "🇻🇳 Tiếng Việt",
            "🇺🇸 English",
            "🇨🇳 中文 (Chinese)",
            "🇫🇷 Français (French)",
            "🇪🇸 Español (Spanish)",
            "🇯🇵 日本語 (Japanese)",
            "🇰🇷 한국어 (Korean)"
        };

        var result = await DisplayActionSheet(AppResources.GetString("Language"), "Hủy", null, languages);

        if (result != null && result != "Hủy")
        {
            string languageCode = GetLanguageCode(result);

            // Update AppResources
            AppResources.SetLanguage(languageCode);
            Preferences.Set("AppLanguage", languageCode);

            // Update label
            UpdateLanguageLabel(languageCode);

            // Reload the app shell to reflect language changes
            var newShell = new AppShell();
            Application.Current.MainPage = newShell;

            if (LanguageLabel != null) LanguageLabel.Text = result;
        }
    }

    private string GetLanguageCode(string displayName)
    {
        return displayName switch
        {
            "🇻🇳 Tiếng Việt" => "vi",
            "🇺🇸 English" => "en",
            "🇨🇳 中文 (Chinese)" => "zh",
            "🇫🇷 Français (French)" => "fr",
            "🇪🇸 Español (Spanish)" => "es",
            "🇯🇵 日本語 (Japanese)" => "ja",
            "🇰🇷 한국어 (Korean)" => "ko",
            _ => "vi"
        };
    }

    private async void OnVoiceClicked(object sender, EventArgs e)
    {
        string[] voices = { "Giọng nữ miền Nam", "Giọng nam miền Bắc", "Giọng nữ miền Bắc", "Giọng nam miền Nam" };
        var result = await DisplayActionSheet("Chọn giọng đọc", "Hủy", null, voices);

        if (result != null && result != "Hủy")
        {
            if (VoiceLabel != null) VoiceLabel.Text = result;
            Preferences.Set("SelectedVoice", result);
        }
    }

    private async void OnRadiusClicked(object sender, EventArgs e)
    {
        string[] radii = { "20 mét", "50 mét", "100 mét", "200 mét" };
        var result = await DisplayActionSheet("Chọn bán kính kích hoạt", "Hủy", null, radii);

        if (result != null && result != "Hủy")
        {
            if (RadiusLabel != null) RadiusLabel.Text = result;
            Preferences.Set("GeoFenceRadius", result);
        }
    }

    private void OnBackgroundTrackingToggled(object sender, ToggledEventArgs e)
    {
        Preferences.Set("BackgroundTracking", e.Value);

        if (e.Value)
        {
            // Enable background tracking
        }
        else
        {
            // Disable background tracking
        }
    }

    private async void OnOfflinePackageClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Tải gói offline", "Tính năng tải gói offline đang được phát triển", "OK");
    }
}
