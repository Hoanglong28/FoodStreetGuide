using System.Diagnostics;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using doanC_.Services.LocationTracking;
using doanC_.Services;
using doanC_.Models;
using Microsoft.Maui.Maps;
using doanC_.Services.Localization;
using Microsoft.Maui.Storage;

namespace doanC_.Views;

public partial class MapPage : ContentPage
{
    private LocationService locationService = new();
    private LocationPointService pointService = new();
    private MapService mapService = new();

    private Location? lastLocation;
    private string _currentLanguage = "vi";

    public MapPage()
    {
        InitializeComponent();
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

            if (ListenLabel != null)
                ListenLabel.Text = AppResources.GetString("TapToListen2");

            if (PlayButton != null)
                PlayButton.Text = AppResources.GetString("PlayCommentary");

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

                        // 🆕 Cập nhật nearby distance
                        int distance = (int)(Location.CalculateDistance(current, lastLocation, DistanceUnits.Kilometers) * 1000);
                        if (NearbyLabel != null)
                            NearbyLabel.Text = string.Format(AppResources.GetString("NearbyDistance"), distance);

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

    private async void OnPlayButtonClicked(object sender, EventArgs e)
    {
        try
        {
      await DisplayAlert(AppResources.GetString("PoiDetail"), AppResources.GetString("FeatureInDevelopment"), AppResources.GetString("OK"));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MapPage] Error: {ex.Message}");
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        locationService.StopTracking();
    }
}