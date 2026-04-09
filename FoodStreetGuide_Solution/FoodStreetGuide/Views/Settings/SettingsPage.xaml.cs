using doanC_.ViewModels;
using doanC_;
using doanC_.Services.Localization;
using doanC_.Services;
using doanC_.Helpers;

namespace doanC_.Views;

public partial class SettingsPage : ContentPage
{
    // ✅ Lấy LocationService từ ServiceHelper (singleton)
    private LocationService _locationService => ServiceHelper.GetService<LocationService>();

    public SettingsPage()
    {
        InitializeComponent();
        this.BindingContext = new SettingsViewModel();
        
        LoadSettings();
    }

    private void LoadSettings()
    {
        var savedLanguage = Preferences.Get("AppLanguage", "vi");
        var savedVoice = Preferences.Get("SelectedVoice", "Giọng nữ");
        var savedRadius = Preferences.Get("GeoFenceRadius", "15 mét");
        var backgroundTracking = Preferences.Get("BackgroundTracking", true);
        var offlinePackage = Preferences.Get("OfflinePackage", "Phố Lê Thánh Tôn · 24MB");

        // Update language label based on saved language code
        UpdateLanguageLabel(savedLanguage);

        // ✅ Dịch giọng sang ngôn ngữ hiện tại
        string displayVoice = savedVoice == "Giọng nữ" ? AppResources.GetString("FemaleVoice") : AppResources.GetString("MaleVoice");
        if (VoiceLabel != null) VoiceLabel.Text = displayVoice;

        // ✅ Dịch bán kính sang ngôn ngữ hiện tại
        string displayRadius = MapRadiusToLocalized(savedRadius);
        if (RadiusLabel != null) RadiusLabel.Text = displayRadius;

        if (BackgroundTrackingSwitch != null) BackgroundTrackingSwitch.IsToggled = backgroundTracking;
        if (OfflinePackageLabel != null) OfflinePackageLabel.Text = offlinePackage;
    }

    private string MapRadiusToLocalized(string savedRadius)
    {
        // Map bán kính từ tiếng Việt sang ngôn ngữ hiện tại
     return savedRadius switch
        {
            "15 mét" => AppResources.GetString("Radius15m"),
    "20 mét" => AppResources.GetString("Radius20m"),
    "25 mét" => AppResources.GetString("Radius25m"),
     "30 mét" => AppResources.GetString("Radius30m"),
  _ => savedRadius // Fallback nếu có giá trị khác
    };
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

        var result = await DisplayActionSheet(AppResources.GetString("Language"), AppResources.GetString("Cancel"), null, languages);

        if (result != null && result != AppResources.GetString("Cancel"))
        {
            string languageCode = GetLanguageCode(result);

            // Update AppResources
            AppResources.SetLanguage(languageCode);
            Preferences.Set("AppLanguage", languageCode);

            // ✅ Thông báo tất cả ViewModels cập nhật ngôn ngữ
            LanguageChangeManager.NotifyLanguageChanged();

            // Update label
            UpdateLanguageLabel(languageCode);

            // ✅ QUAN TRỌNG: Reload AppShell để tất cả trang được cập nhật ngôn ngữ
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
        // ✅ Dịch tiêu đề và các tùy chọn giọng theo ngôn ngữ hiện tại
        string voiceTitle = AppResources.GetString("VoiceTTSLabel");
        string cancelText = AppResources.GetString("Cancel");

        // Lấy giọng từ AppResources theo ngôn ngữ hiện tại
        string maleVoice = AppResources.GetString("MaleVoice");
        string femaleVoice = AppResources.GetString("FemaleVoice");

        string[] voices = { femaleVoice, maleVoice };
        var result = await DisplayActionSheet(voiceTitle, cancelText, null, voices);

        if (result != null && result != cancelText)
        {
            // ✅ Map từ ngôn ngữ hiện tại về "Giọng nữ" hoặc "Giọng nam" (tiếng Việt) để lưu
            string voiceToSave = result == femaleVoice ? "Giọng nữ" : "Giọng nam";

            Preferences.Set("SelectedVoice", voiceToSave);

            // ✅ Set text cho VoiceLabel
            if (VoiceLabel != null) VoiceLabel.Text = result;

            // ✅ Refresh binding
            (this.BindingContext as SettingsViewModel)?.RefreshLanguage();
        }
    }

    private async void OnRadiusClicked(object sender, EventArgs e)
    {
        // ✅ Dịch các tùy chọn bán kính theo ngôn ngữ hiện tại
        string radius15 = AppResources.GetString("Radius15m");
        string radius20 = AppResources.GetString("Radius20m");
        string radius25 = AppResources.GetString("Radius25m");
        string radius30 = AppResources.GetString("Radius30m");

        string[] radii = { radius15, radius20, radius25, radius30 };
        var result = await DisplayActionSheet(AppResources.GetString("RadiusActivationLabel"), AppResources.GetString("Cancel"), null, radii);

        if (result != null && result != AppResources.GetString("Cancel"))
        {
            // ✅ Set text cho RadiusLabel
            if (RadiusLabel != null) RadiusLabel.Text = result;
            Preferences.Set("GeoFenceRadius", result);
        }
    }

    private void OnBackgroundTrackingToggled(object sender, ToggledEventArgs e)
    {
        // ✅ Lưu trạng thái Background Tracking vào Preferences
        Preferences.Set("BackgroundTracking", e.Value);

        if (e.Value)
        {
     // ✅ Bật theo dõi nền - dùng pin nhiều hơn
            System.Diagnostics.Debug.WriteLine("[SettingsPage] Background Tracking: ON ✅");
            System.Diagnostics.Debug.WriteLine("[SettingsPage] Sẽ theo dõi vị trí liên tục, tiêu thụ pin nhiều hơn");
           
 // ✅ Bắt đầu theo dõi vị trí nền
  StartBackgroundTracking();
   }
        else
        {
    // ✅ Tắt theo dõi nền - tiết kiệm pin
  System.Diagnostics.Debug.WriteLine("[SettingsPage] Background Tracking: OFF ❌");
      System.Diagnostics.Debug.WriteLine("[SettingsPage] Chỉ theo dõi vị trí khi app mở");
           
         // ✅ Dừng theo dõi nền
            StopBackgroundTracking();
        }
    }

    private void StartBackgroundTracking()
    {
        try
    {
System.Diagnostics.Debug.WriteLine("[SettingsPage] 🔄 Bắt đầu theo dõi nền...");
   
         // ✅ Gọi LocationService để bắt đầu theo dõi
   _ = _locationService.StartTrackingAsync(location =>
            {
      MainThread.BeginInvokeOnMainThread(() =>
   {
 System.Diagnostics.Debug.WriteLine($"[SettingsPage] 📍 Location updated: {location.Latitude}, {location.Longitude}");
    // Cập nhật vị trí nếu cần
          });
      });
           
   System.Diagnostics.Debug.WriteLine("[SettingsPage] ✅ Background Tracking bắt đầu");
        }
        catch (Exception ex)
        {
      System.Diagnostics.Debug.WriteLine($"[SettingsPage] ❌ Lỗi khi bắt đầu tracking: {ex.Message}");
        }
    }

    private void StopBackgroundTracking()
  {
   try
    {
      System.Diagnostics.Debug.WriteLine("[SettingsPage] 🛑 Dừng theo dõi nền...");
           
    // ✅ Gọi LocationService để dừng theo dõi
       _locationService.StopTracking();
  
    System.Diagnostics.Debug.WriteLine("[SettingsPage] ✅ Background Tracking dừng");
   }
        catch (Exception ex)
        {
        System.Diagnostics.Debug.WriteLine($"[SettingsPage] ❌ Lỗi khi dừng tracking: {ex.Message}");
        }
    }

    private async void OnOfflinePackageClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Tải gói offline", "Tính năng tải gói offline đang được phát triển", "OK");
    }
}
