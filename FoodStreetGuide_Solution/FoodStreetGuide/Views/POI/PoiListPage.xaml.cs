using System.Diagnostics;
using doanC_.Helpers;
using doanC_.Models;
using doanC_.Services.Api;           // ← THÊM DÒNG NÀY
using doanC_.Services;
using doanC_.Services.Localization;
using doanC_.Services.Audio;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Storage;
using doanC_.ViewModels;

namespace doanC_.Views;

public partial class PoiListPage : ContentPage
{
    private ApiService _apiService;                     // ← ĐỔI từ SQLiteService
    private LocationService _locationService;
    private LibreTranslateService _translationService;
    private TTSService _ttsService;
    private List<LocationPoint> _allLocationPoints;
    private Location _currentLocation;

    public PoiListPage()
    {
        InitializeComponent();

        // Dùng ViewModel cho text + localization
        BindingContext = new PoiListViewModel();

        _apiService = new ApiService();                  // ← DÙNG API SERVICE
        _translationService = ServiceHelper.GetService<LibreTranslateService>();
        _ttsService = ServiceHelper.GetService<TTSService>();
        _locationService = new LocationService();

        LoadDataFromApi();                               // ← ĐỔI TÊN HÀM
        GetCurrentLocationAndCalculateDistance();
    }

    private async void GetCurrentLocationAndCalculateDistance()
    {
        try
        {
            _currentLocation = await _locationService.GetCurrentLocationAsync();

            if (_currentLocation != null)
            {
                Debug.WriteLine($"[PoiListPage] Current location: {_currentLocation.Latitude}, {_currentLocation.Longitude}");
                RefreshDistances();
            }
            else
            {
                Debug.WriteLine("[PoiListPage] Unable to get current location");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[PoiListPage] Error getting location: {ex.Message}");
        }
    }

    private async void LoadDataFromApi()                  // ← ĐỔI TÊN HÀM
    {
        try
        {
            // Hiển thị loading
            loadingIndicator.IsVisible = true;
            loadingIndicator.IsRunning = true;

            _allLocationPoints = await _apiService.GetLocationPointsAsync();

            if (_allLocationPoints != null && _allLocationPoints.Any())
            {
                var poiItems = _allLocationPoints.Select(location => new PoiItem
                {
                    PointId = location.PointId,           // ← ĐỔI Id → PointId
                    Name = location.Name,
                    Description = location.Description,
                    Distance = 0,
                    ImageUrl = location.Image,
                    Category = location.Category,
                    Rating = location.Rating,
                    ReviewCount = location.ReviewCount,
                    Latitude = location.Latitude,
                    Longitude = location.Longitude
                }).ToList();

                PoiCollection.ItemsSource = poiItems;

                Debug.WriteLine($"[PoiListPage] Loaded {poiItems.Count} POI items from API");
            }
            else
            {
                Debug.WriteLine("[PoiListPage] No data from API");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[PoiListPage] Error loading data: {ex.Message}");
            var errorMsg = AppResources.GetString("CannotLoadData");
            await DisplayAlert(AppResources.GetString("Error"), $"{errorMsg}: {ex.Message}", AppResources.GetString("OK"));
        }
        finally
        {
            loadingIndicator.IsVisible = false;
            loadingIndicator.IsRunning = false;
        }
    }

    private void RefreshDistances()
    {
        if (_currentLocation == null || _allLocationPoints == null)
            return;

        var updatedItems = _allLocationPoints.Select(location => new PoiItem
        {
            PointId = location.PointId,                   // ← ĐỔI Id → PointId
            Name = location.Name,
            Description = location.Description,
            Distance = (int)CalculateDistance(_currentLocation.Latitude, _currentLocation.Longitude, location.Latitude, location.Longitude),
            ImageUrl = location.Image,
            Category = location.Category,
            Rating = location.Rating,
            ReviewCount = location.ReviewCount,
            Latitude = location.Latitude,
            Longitude = location.Longitude
        }).ToList();

        PoiCollection.ItemsSource = updatedItems;
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        SearchData(e.NewTextValue);
    }

    private void SearchData(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            RefreshDistances();
        }
        else
        {
            var filtered = _allLocationPoints
                .Where(l => l.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                            (l.Description != null && l.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase)))
                .Select(location => new PoiItem
                {
                    PointId = location.PointId,           // ← ĐỔI Id → PointId
                    Name = location.Name,
                    Description = location.Description,
                    Distance = _currentLocation != null ? (int)CalculateDistance(_currentLocation.Latitude, _currentLocation.Longitude, location.Latitude, location.Longitude) : 0,
                    ImageUrl = location.Image,
                    Category = location.Category,
                    Rating = location.Rating,
                    ReviewCount = location.ReviewCount,
                    Latitude = location.Latitude,
                    Longitude = location.Longitude
                }).ToList();

            PoiCollection.ItemsSource = filtered;
        }
    }

    private async void OnPoiTapped(object sender, TappedEventArgs e)
    {
        try
        {
            var frame = sender as Frame;
            if (frame?.BindingContext is PoiItem selectedPoi)
            {
                Debug.WriteLine($"[PoiListPage] Selected POI: {selectedPoi.Name} (ID: {selectedPoi.PointId})");
                await Shell.Current.GoToAsync($"///poidetailpage?poiId={selectedPoi.PointId}");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[PoiListPage] Error: {ex.Message}");
        }
    }

    private async void OnPlayButtonTapped(object sender, EventArgs e)
    {
        try
        {
            if (sender is Button button)
            {
                var frame = button.Parent?.Parent?.Parent as Frame;

                if (frame?.BindingContext is PoiItem selectedPoi)
                {
                    await HandlePlayButton(selectedPoi);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[PoiListPage] ❌ Error in OnPlayButtonTapped: {ex.Message}");
        }
    }

    private async Task HandlePlayButton(PoiItem selectedPoi)
    {
        try
        {
            if (selectedPoi == null)
            {
                Debug.WriteLine("[PoiListPage] ❌ SelectedPoi is null");
                return;
            }

            Debug.WriteLine($"\n[PoiListPage] 📝 Play button tapped for: {selectedPoi.Name}");

            var locationPoint = _allLocationPoints.FirstOrDefault(l => l.PointId == selectedPoi.PointId);  // ← ĐỔI Id → PointId

            if (locationPoint == null)
            {
                Debug.WriteLine($"[PoiListPage] ❌ LocationPoint not found for ID: {selectedPoi.PointId}");
                return;
            }

            // Lấy ngôn ngữ từ Preferences
            var savedLanguage = Preferences.Get("AppLanguage", "vi");
            Debug.WriteLine($"[PoiListPage] 🌐 Using saved language: {savedLanguage}");

            // ✅ Lấy giọng đã chọn từ Preferences
            var selectedVoice = Preferences.Get("SelectedVoice", "Giọng nữ");
            Debug.WriteLine($"[PoiListPage] 👤 Selected Voice: {selectedVoice}");

            Debug.WriteLine($"[PoiListPage] Original text:");
            Debug.WriteLine($"  📍 Name: {locationPoint.Name}");
            Debug.WriteLine($"  📍 Description: {locationPoint.Description}");

            // Dịch Description sang ngôn ngữ đã chọn
            var translatedDescription = await _translationService.TranslateTextAsync(
                locationPoint.Description ?? locationPoint.Name,
                savedLanguage
            );

            Debug.WriteLine($"\n[PoiListPage] ✅ Translated description:");
            Debug.WriteLine($"  🌍 {translatedDescription}");

            // Phát âm thanh dịch với giọng đã chọn
            Debug.WriteLine($"[PoiListPage] 🔊 Speaking translated text...");

            try
            {
                await _ttsService.SpeakAsync(translatedDescription, GetLanguageCodeForTTS(savedLanguage), selectedVoice);
                Debug.WriteLine("[PoiListPage] ✅ Speech completed");
            }
            catch (Exception ttsEx)
            {
                Debug.WriteLine($"[PoiListPage] ❌ TextToSpeech Exception: {ttsEx.Message}");
                Debug.WriteLine($"[PoiListPage] ❌ Stack: {ttsEx.StackTrace}");

                // Fallback: Display text if TTS fails
                await DisplayAlert(
                    $"Thuyết minh: {locationPoint.Name}",
                    $"{translatedDescription}",
                    AppResources.GetString("OK")
                );
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[PoiListPage] ❌ Error in HandlePlayButton: {ex.Message}");
            Debug.WriteLine($"[PoiListPage] ❌ Stack trace: {ex.StackTrace}");

            await DisplayAlert(AppResources.GetString("Error"), $"Error: {ex.Message}", AppResources.GetString("OK"));
        }
    }

    // Helper: Convert language code to TTS locale
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
            _ => "vi-VN"
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

public class PoiItem
{
    public int PointId { get; set; }           // ← ĐỔI Id → PointId
    public string Name { get; set; }
    public string Description { get; set; }
    public int Distance { get; set; }
    public string ImageUrl { get; set; }
    public string Category { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}