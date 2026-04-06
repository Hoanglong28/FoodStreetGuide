using System.Diagnostics;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using doanC_.Services.LocationTracking;
using doanC_.Services;
using doanC_.Models;
using Microsoft.Maui.Maps;

namespace doanC_.Views;

public partial class MapPage : ContentPage
{
    private LocationService locationService = new();
    private LocationPointService pointService = new();
    private MapService mapService = new();

    private Location? lastLocation;

    public MapPage()
    {
        InitializeComponent();
        Loaded += async (s, e) => await LoadMap();
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

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        locationService.StopTracking();
    }
}