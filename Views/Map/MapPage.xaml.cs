using AndroidX.Lifecycle;
using doanC_.Helpers;
using doanC_.Models;
using doanC_.Services;
using doanC_.Services.Data;
using doanC_.Services.Localization;
using doanC_.Services.LocationTracking;
using doanC_.Services.Audio;
using doanC_.ViewModels;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Storage;
using System.Diagnostics;

namespace doanC_.Views;

public partial class MapPage : ContentPage
{
    private LocationService locationService = new();
    private LocationPointService pointService = new();
    private MapService mapService = new();
    private SQLiteService _sqliteService;
    private TTSService _ttsService;
    private LibreTranslateService _translationService;

    private Location? lastLocation;
    private string _currentLanguage = "vi";
    private MapViewModel _viewModel = new();

    public MapPage()
    {
        InitializeComponent();
        
        // ✅ Gán BindingContext để XAML binding hoạt động
        BindingContext = _viewModel;
    
        _sqliteService = ServiceHelper.GetService<SQLiteService>();
        _ttsService = ServiceHelper.GetService<TTSService>();
        _translationService = ServiceHelper.GetService<LibreTranslateService>();
    
        Loaded += async (s, e) => await LoadMap();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // 🆕 Tải lại ngôn ngữ mỗi khi trang được hiển thị
        LoadLanguage();
    }

    /// <summary>
    /// Cập nhật UI theo ngôn ngữ đã chọn
    /// </summary>
  private void LoadLanguage()
    {
     try
   {
       var savedLanguage = Preferences.Get("AppLanguage", "vi");
            _currentLanguage = savedLanguage;

            if (SearchEntry != null)
  SearchEntry.Placeholder = AppResources.GetString("FindPoiPlaceholder");

          if (LanguageLabel != null)
        LanguageLabel.Text = GetLanguageDisplay(_currentLanguage);

     Debug.WriteLine($"[MapPage] 🌐 UI loaded in language: {_currentLanguage}");
        }
        catch (Exception ex)
  {
    Debug.WriteLine($"[MapPage] Error loading language: {ex.Message}");
        }
    }

    /// <summary>
    /// Chuyển đổi language code sang display name
    /// </summary>
    private string GetLanguageDisplay(string languageCode)
  {
     return languageCode switch
     {
            "vi" => "VI",
 "en" => "EN",
   "fr" => "FR",
            "zh" => "ZH",
            "es" => "ES",
            "ja" => "JA",
     "ko" => "KO",
 _ => "VI"
        };
    }

    private async Task LoadMap()
    {
        try
  {
       var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            // set default center first
            var defaultLocation = new Location(10.7726, 106.6980); // TP.HCM
       map.MoveToRegion(
      MapSpan.FromCenterAndRadius(
        defaultLocation,
        Distance.FromMeters(200)));

     // ✅ Load POI từ database vào ViewModel
    await LoadPoiFromDatabase();

            // Load POI from SQLite via MapService before starting tracking
            await mapService.AddLocationPointsFromDbAsync(map);
            Debug.WriteLine($"[MapPage] Map pins count after DB load: {map.Pins.Count}");

   if (status == PermissionStatus.Granted)
    {
           // Start tracking in background (do NOT await) so LoadMap can finish
    _ = locationService.StartTrackingAsync(location =>
         {
     MainThread.BeginInvokeOnMainThread(async () =>
          {
       var current = new Location(location.Latitude, location.Longitude);

         if (lastLocation != null &&
           Location.CalculateDistance(lastLocation, current, DistanceUnits.Kilometers) * 1000 < 10)
      {
     return;
    }

 lastLocation = current;

          // 🆕 Cập nhật POI gần nhất
           _viewModel.UpdateNearestPoi(current);

             double zoom = 100;
      if (location.Speed.HasValue && location.Speed > 5)
            zoom = 200;
  else
  zoom = 80;

           await Task.Delay(30);

  map.MoveToRegion(
 MapSpan.FromCenterAndRadius(
          current,
         Distance.FromMeters(zoom)));
       });
   });
            }
        }
 catch (Exception ex)
    {
    Debug.WriteLine(ex.Message);
    }
    }

    /// <summary>
    /// Load POI từ database vào ViewModel
/// </summary>
    private async Task LoadPoiFromDatabase()
    {
        try
        {
            var poiList = await _sqliteService.GetAllLocationPointsAsync();
  
    Debug.WriteLine($"[MapPage] Database POI count: {poiList?.Count ?? 0}");
   
      if (poiList != null && poiList.Count > 0)
            {
        _viewModel.Points.Clear();
          foreach (var poi in poiList)
      {
         _viewModel.Points.Add(poi);
     Debug.WriteLine($"[MapPage] Added POI: {poi.Name} ({poi.Latitude}, {poi.Longitude})");
  }
        Debug.WriteLine($"[MapPage] ✅ Loaded {poiList.Count} POI from database into ViewModel");
  
    // ✅ Nếu đã có POI và có location, cập nhật ngay
  if (lastLocation != null)
    {
   _viewModel.UpdateNearestPoi(lastLocation);
        Debug.WriteLine($"[MapPage] Updated nearest POI: {_viewModel.PoiName}");
     }
      }
  else
            {
            Debug.WriteLine($"[MapPage] ⚠️ No POI found in database");
            }
     }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MapPage] ❌ Error loading POI: {ex.Message}");
      Debug.WriteLine($"[MapPage] ❌ Stack: {ex.StackTrace}");
      }
    }

    private async void OnPlayButtonClicked(object sender, EventArgs e)
    {
        try
        {
   // ✅ Kiểm tra có POI gần nhất không
   if (_viewModel.NearestPoi == null)
      {
       await DisplayAlert(
        AppResources.GetString("Error"),
           "Không có điểm nào gần bạn",
      AppResources.GetString("OK"));
         return;
            }

 var nearestPoi = _viewModel.NearestPoi;
            string poiName = nearestPoi.Name;
            string poiDescription = nearestPoi.Description ?? poiName;

     Debug.WriteLine($"[MapPage] 🔊 Playing audio for: {poiName}");
            Debug.WriteLine($"[MapPage] 📝 Description: {poiDescription}");
        Debug.WriteLine($"[MapPage] 🌐 Language: {_currentLanguage}");

  // ✅ Dịch mô tả sang ngôn ngữ đã chọn
       string textToSpeak = poiDescription;
         if (_currentLanguage != "vi" && _translationService != null)
   {
              try
                {
             Debug.WriteLine($"[MapPage] 🔄 Translating to {_currentLanguage}...");
        textToSpeak = await _translationService.TranslateTextAsync(poiDescription, _currentLanguage);
       Debug.WriteLine($"[MapPage] ✅ Translated: {textToSpeak}");
    }
      catch (Exception transEx)
{
             Debug.WriteLine($"[MapPage] ⚠️ Translation failed, using original: {transEx.Message}");
      textToSpeak = poiDescription;
       }
    }

         // ✅ Phát âm thanh 
       if (_ttsService != null)
      {
     try
  {
              Debug.WriteLine($"[MapPage] 🔊 Speaking: {textToSpeak}");
         await _ttsService.SpeakAsync(textToSpeak, GetLanguageCodeForTTS(_currentLanguage));
         Debug.WriteLine($"[MapPage] ✅ Speech completed");
    }
      catch (Exception ttsEx)
      {
        Debug.WriteLine($"[MapPage] ❌ TTS Error: {ttsEx.Message}");
     
       // Fallback: Hiển thị alert nếu TTS fails
                  await DisplayAlert(
            $"📍 {poiName}",
           $"{textToSpeak}",
            AppResources.GetString("OK"));
        }
            }
       else
            {
       Debug.WriteLine($"[MapPage] ❌ TTSService is null");
                // Hiển thị alert nếu không có TTS service
    await DisplayAlert(
          $"📍 {poiName}",
          $"{textToSpeak}",
        AppResources.GetString("OK"));
   }
        }
        catch (Exception ex)
        {
 Debug.WriteLine($"[MapPage] ❌ Error in OnPlayButtonClicked: {ex.Message}");
Debug.WriteLine($"[MapPage] ❌ Stack: {ex.StackTrace}");
 
   await DisplayAlert(
  AppResources.GetString("Error"),
    $"Lỗi phát âm thanh: {ex.Message}",
     AppResources.GetString("OK"));
        }
    }

    /// <summary>
    /// Helper: Convert language code to TTS locale
    /// </summary>
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
        "ko" => "ko-KR",
          _ => "vi-VN"
        };
    }

    protected override void OnDisappearing()
    {
 base.OnDisappearing();
        locationService.StopTracking();
    }
}