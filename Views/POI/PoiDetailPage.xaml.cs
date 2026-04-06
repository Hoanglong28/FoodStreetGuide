using System.Diagnostics;
using doanC_.Helpers;
using doanC_.Models;
using doanC_.Services.Data;
using doanC_.Services;
using doanC_.Services.Localization;
using doanC_.Services.Audio;
using doanC_.ViewModels;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Storage;

namespace doanC_.Views;

[QueryProperty(nameof(PoiId), "poiId")]
public partial class PoiDetailPage : ContentPage
{
    public PoiDetailPage()
    {
        InitializeComponent();
        this.BindingContext = new PoiDetailViewModel();
  _sqliteService = ServiceHelper.GetService<SQLiteService>();
  _translationService = ServiceHelper.GetService<LibreTranslateService>();
        _ttsService = ServiceHelper.GetService<TTSService>();
   _locationService = new LocationService();
  
   LoadSavedLanguage();
    }

    private SQLiteService _sqliteService;
    private LocationService _locationService;
    private LibreTranslateService _translationService;
    private TTSService _ttsService;
    private LocationPoint _currentPoi;
    private Location _userLocation;
    private bool _isAudioPlayerVisible = false;
    private string _currentLanguage = "vi";
    private bool _isPlaying = false;
    private string _originalDescription = "";

    private int _poiId;
    public int PoiId
    {
        get => _poiId;
        set
        {
            _poiId = value;
            _ = LoadPoiDetailsAsync(_poiId);
        }
    }

    /// <summary>
    /// Tải ngôn ngữ đã chọn từ Preferences
    /// </summary>
    private void LoadSavedLanguage()
    {
        var savedLanguage = Preferences.Get("AppLanguage", "vi");
  _currentLanguage = savedLanguage;
        
        Debug.WriteLine($"[PoiDetailPage] 🌐 Loaded saved language: {_currentLanguage}");
        
  if (_translationService != null)
    {
            _translationService.SetLanguage(_currentLanguage);
        }
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
     base.OnNavigatedTo(args);
        GetCurrentLocationAndCalculateDistance();
    }

    private async Task LoadPoiDetailsAsync(int poiId)
    {
   try
        {
     if (poiId == 0)
   {
    Debug.WriteLine("[PoiDetailPage] Invalid POI ID: 0");
        return;
 }

     Debug.WriteLine($"[PoiDetailPage] Loading POI with ID: {poiId}");

     _currentPoi = await _sqliteService.GetLocationPointByIdAsync(poiId);

            if (_currentPoi == null)
     {
      Debug.WriteLine($"[PoiDetailPage] POI not found with ID: {poiId}");
    await DisplayAlert(AppResources.GetString("Error"), AppResources.GetString("NotFound"), AppResources.GetString("OK"));
              await GoBackAsync();
   return;
    }

            UpdatePoiUI();

    // 🆕 Nếu ngôn ngữ không phải tiếng Việt, tự động dịch mô tả
  if (_currentLanguage != "vi")
         {
 await TranslateDescriptionAsync();
        }

   _userLocation = await _locationService.GetCurrentLocationAsync();
         if (_userLocation != null)
            {
     UpdateDistance();
          }

    Debug.WriteLine($"[PoiDetailPage] Successfully loaded POI: {_currentPoi.Name}");
      }
     catch (Exception ex)
        {
 Debug.WriteLine($"[PoiDetailPage] Error loading POI: {ex.Message}");
            await DisplayAlert(AppResources.GetString("Error"), $"{AppResources.GetString("CannotLoadData")}: {ex.Message}", AppResources.GetString("OK"));
  await GoBackAsync();
    }
    }

    /// <summary>
    /// Tự động dịch mô tả sang ngôn ngữ đã chọn
    /// </summary>
    private async Task TranslateDescriptionAsync()
    {
try
   {
            if (string.IsNullOrEmpty(_originalDescription) || _currentLanguage == "vi")
       return;

         await MainThread.InvokeOnMainThreadAsync(() =>
{
        DescriptionLabel.Text = AppResources.GetString("Translating");
            });

 var translatedText = await _translationService.TranslateTextAsync(_originalDescription, _currentLanguage);

      Debug.WriteLine($"[PoiDetailPage] 📝 Dịch sang {_currentLanguage}: {translatedText}");

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
    DescriptionLabel.Text = translatedText;
   });
   }
     catch (Exception ex)
   {
       Debug.WriteLine($"[PoiDetailPage] ❌ Error translating: {ex.Message}");
            DescriptionLabel.Text = _originalDescription;
    }
    }

    private void UpdatePoiUI()
  {
        Debug.WriteLine("[PoiDetailPage] ✅ UpdatePoiUI called");

        if (_currentPoi == null)
        {
            Debug.WriteLine("[PoiDetailPage] _currentPoi is null in UpdatePoiUI");
    return;
    }

        try
        {
     Debug.WriteLine("[PoiDetailPage] Starting UpdatePoiUI - About to update all controls");

          Title = _currentPoi.Name;
       MainImage.Source = _currentPoi.Image ?? "poi_default.png";
  PoiNameLabel.Text = _currentPoi.Name;

  // ✅ Rating
            if (RatingLabel != null)
            {
      RatingLabel.Text = $"★ {_currentPoi.Rating} ({_currentPoi.ReviewCount})";
      Debug.WriteLine($"[PoiDetailPage] ✅ RatingLabel.Text = {RatingLabel.Text}");
       }

CategoryLabel.Text = (_currentPoi.Category ?? AppResources.GetString("UncategorizedCategory")).ToUpper();
            AddressLabel.Text = _currentPoi.Address ?? AppResources.GetString("Unknown");
       
        // 🆕 Lưu mô tả gốc
       _originalDescription = _currentPoi.Description ?? AppResources.GetString("NoDescription");
 DescriptionLabel.Text = _originalDescription;

  OpeningHoursLabel.Text = string.IsNullOrEmpty(_currentPoi.OpeningHours)
              ? AppResources.GetString("OpeningHours")
                : $"⏰ {_currentPoi.OpeningHours}";
// ✅ Thời gian mở cửa
// OpeningHoursLabel.Text = string.IsNullOrEmpty(_currentPoi.OpeningHours)
//     ? AppResources.GetString("DefaultOpeningHours")
//     : $"⏰ {_currentPoi.OpeningHours}";

            // ✅ Cập nhật giá
     if (PriceLabel != null)
     {
                PriceLabel.Text = GetPriceRangeDisplay(_currentPoi.PriceRange);
                PriceLabel.IsVisible = true;
    Debug.WriteLine($"[PoiDetailPage] Price updated: {PriceLabel.Text}");
            }

        Debug.WriteLine($"[PoiDetailPage] ✅ UI updated successfully");
        }
        catch (Exception ex)
   {
            Debug.WriteLine($"[PoiDetailPage] ❌❌❌ Error in UpdatePoiUI: {ex.Message}");
     Debug.WriteLine($"[PoiDetailPage] Stack trace: {ex.StackTrace}");
   }
    }

    /// <summary>
    /// Ánh xạ PriceRange từ database sang giá tiền thực tế
    /// </summary>
    private string GetPriceRangeDisplay(string priceRange)
    {
        if (string.IsNullOrEmpty(priceRange))
      return AppResources.GetString("ContactForPrice");

        return priceRange.ToLower() switch
  {
            "rẻ" => "💰 10.000 – 50.000đ",
"trung bình" => "💰 70.000 – 150.000đ",
            "cao" => "💰 150.000 – 300.000đ",
            _ => $"💰 {priceRange}"
 };
    }

    private void UpdateDistance()
    {
        if (_userLocation == null || _currentPoi == null)
  return;

        try
        {
  double distance = CalculateDistance(
    _userLocation.Latitude,
        _userLocation.Longitude,
                _currentPoi.Latitude,
            _currentPoi.Longitude
            );
            Debug.WriteLine($"[PoiDetailPage] Distance: {(int)distance}m");
        }
     catch (Exception ex)
        {
            Debug.WriteLine($"[PoiDetailPage] Error calculating distance: {ex.Message}");
        }
    }

    private async void GetCurrentLocationAndCalculateDistance()
    {
        try
   {
     _userLocation = await _locationService.GetCurrentLocationAsync();
      if (_userLocation != null)
        {
       UpdateDistance();
       }
        }
   catch (Exception ex)
     {
    Debug.WriteLine($"[PoiDetailPage] Error getting location: {ex.Message}");
  }
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
{
        try
        {
            Debug.WriteLine("[PoiDetailPage] Back button clicked");
            await GoBackAsync();
        }
    catch (Exception ex)
     {
            Debug.WriteLine($"[PoiDetailPage] Error going back: {ex.Message}");
      }
    }

    private async Task GoBackAsync()
    {
    try
 {
          if (Shell.Current.Navigation.NavigationStack.Count > 1)
   {
         Debug.WriteLine("[PoiDetailPage] Using relative back navigation");
 await Shell.Current.GoToAsync("..");
            }
            else
            {
    Debug.WriteLine("[PoiDetailPage] Navigation stack empty, going to PoiListPage");
         await Shell.Current.GoToAsync("///PoiListPage");
     }
        }
        catch (Exception ex)
   {
            Debug.WriteLine($"[PoiDetailPage] GoBackAsync error: {ex.Message}");
      }
    }

    private async void OnFavoriteClicked(object sender, EventArgs e)
    {
        try
        {
       await DisplayAlert(AppResources.GetString("Favorite"), AppResources.GetString("AddedToFavorite"), AppResources.GetString("OK"));
        }
    catch (Exception ex)
        {
            Debug.WriteLine($"[PoiDetailPage] Error: {ex.Message}");
        }
    }

    private async void OnShareClicked(object sender, EventArgs e)
    {
    try
        {
         if (_currentPoi != null)
     {
                await Share.Default.RequestAsync(new ShareTextRequest
       {
      Text = $"{AppResources.GetString("PoiDetail")}: {_currentPoi.Name} at {_currentPoi.Address}",
           Title = _currentPoi.Name
           });
     }
  }
        catch (Exception ex)
        {
    Debug.WriteLine($"[PoiDetailPage] Error sharing: {ex.Message}");
     }
    }

    private async void OnOpenStatusClicked(object sender, EventArgs e)
    {
        await DisplayAlert(AppResources.GetString("Information"), AppResources.GetString("OpenInfo"), AppResources.GetString("OK"));
    }

    private void OnPlayAudioClicked(object sender, EventArgs e)
    {
        try
        {
            _isAudioPlayerVisible = !_isAudioPlayerVisible;
      AudioPlayerFrame.IsVisible = _isAudioPlayerVisible;
      }
        catch (Exception ex)
        {
      Debug.WriteLine($"[PoiDetailPage] Error: {ex.Message}");
    }
    }

    private async void OnGetDirectionsClicked(object sender, EventArgs e)
    {
        try
        {
            if (_currentPoi == null)
 return;

            var mapUrl = $"https://www.google.com/maps/search/?api=1&query={_currentPoi.Latitude},{_currentPoi.Longitude}";
            await Launcher.Default.OpenAsync(mapUrl);
   }
        catch (Exception ex)
        {
            Debug.WriteLine($"[PoiDetailPage] Error opening maps: {ex.Message}");
     await DisplayAlert(AppResources.GetString("Error"), AppResources.GetString("CannotOpenMap"), AppResources.GetString("OK"));
        }
    }

    private async void OnLanguageChanged(object sender, EventArgs e)
    {
        try
        {
      if (AudioLanguagePicker?.SelectedIndex < 0 || _currentPoi == null)
     return;

            var selectedLanguage = AudioLanguagePicker.Items[AudioLanguagePicker.SelectedIndex];
  _currentLanguage = GetLanguageCode(selectedLanguage);

        Debug.WriteLine($"[PoiDetailPage] 🌐 Language changed to: {selectedLanguage} (Code: {_currentLanguage})");

  await TranslateDescriptionAsync();
        }
     catch (Exception ex)
        {
            Debug.WriteLine($"[PoiDetailPage] ❌ Error in OnLanguageChanged: {ex.Message}");
 await DisplayAlert(AppResources.GetString("Error"), $"{AppResources.GetString("TranslationError")}: {ex.Message}", AppResources.GetString("OK"));
        }
    }

    private async void OnPlayPauseClicked(object sender, EventArgs e)
    {
        try
     {
         if (_currentPoi == null || PlayPauseButton == null)
           return;

  if (!_isPlaying)
    {
    _isPlaying = true;
     PlayPauseButton.Text = AppResources.GetString("Stop");

      var textToSpeak = DescriptionLabel?.Text ?? _originalDescription ?? _currentPoi.Name;

     if (string.IsNullOrEmpty(textToSpeak))
            {
    Debug.WriteLine($"[PoiDetailPage] ❌ No text to speak");
      _isPlaying = false;
          PlayPauseButton.Text = AppResources.GetString("Play");
           return;
        }

      Debug.WriteLine($"[PoiDetailPage] 🔊 Playing: {textToSpeak}");
     Debug.WriteLine($"[PoiDetailPage] 📢 Language: {_currentLanguage}");

      await _ttsService.SpeakAsync(textToSpeak, GetLanguageCodeForTTS(_currentLanguage));

         Debug.WriteLine($"[PoiDetailPage] ✅ Playback completed");

        _isPlaying = false;
     PlayPauseButton.Text = AppResources.GetString("Play");
            }
     else
            {
     Debug.WriteLine($"[PoiDetailPage] ⏹️ Canceling speech");
           _isPlaying = false;
       PlayPauseButton.Text = AppResources.GetString("Play");
await _ttsService.CancelAsync();
         }
    }
        catch (Exception ex)
        {
     Debug.WriteLine($"[PoiDetailPage] ❌ Error in OnPlayPauseClicked: {ex.Message}");
      Debug.WriteLine($"[PoiDetailPage] ❌ Stack trace: {ex.StackTrace}");
            PlayPauseButton.Text = AppResources.GetString("Play");
_isPlaying = false;
       await DisplayAlert(AppResources.GetString("Error"), $"{AppResources.GetString("SpeechError")}: {ex.Message}", AppResources.GetString("OK"));
        }
    }

    private string GetLanguageCode(string languageName)
    {
        if (string.IsNullOrEmpty(languageName))
    return "vi";
        return languageName.ToLower() switch
        {
    "tiếng việt" or "vietnamese" => "vi",
     "tiếng anh" or "english" => "en",
    "tiếng pháp" or "french" => "fr",
 "tiếng tây ban nha" or "spanish" => "es",
   "tiếng trung" or "中文" => "zh",
          "tiếng nhật" or "日本語" => "ja",
            "tiếng hàn" or "한국어" => "ko",
        _ => "vi"
        };
    }

    private string GetLanguageCodeForTTS(string languageCode)
    {
return languageCode switch
     {
    "en" => "en-US",
 "fr" => "fr-FR",
      "es" => "es-ES",
            "zh" => "zh-CN",
            "ja" => "ja-JP",
   "vi" => "vi-VN",
            _ => "en-US"
      };
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
   double R = 6371000;
      double dLat = ToRad(lat2 - lat1);
        double dLon = ToRad(lon2 - lon1);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
       Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
          Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
      return R * c;
 }

    private double ToRad(double val) => val * Math.PI / 180;
}