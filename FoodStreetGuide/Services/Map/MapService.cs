using System.Diagnostics;
using doanC_.Models;
using doanC_.Services.Data;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;

// alias để tránh conflict
using MapControl = Microsoft.Maui.Controls.Maps.Map;

namespace doanC_.Services;

public class MapService
{
    private readonly SQLiteService _sqlite = new();

    // Focus bản đồ
    public void FocusToLocation(MapControl map, double latitude, double longitude)
    {
        var location = new Location(latitude, longitude);

        map.MoveToRegion(
            MapSpan.FromCenterAndRadius(
                location,
                Distance.FromMeters(500)));
    }

    // Hiển thị vị trí user
    public void ShowUserLocation(MapControl map, double latitude, double longitude)
    {
        var location = new Location(latitude, longitude);

        var pin = new Pin
        {
            Label = "Bạn đang ở đây",
            Location = location,
            Type = PinType.SavedPin
        };

        map.Pins.Add(pin);
    }

    // Cập nhật vị trí user
    private Pin? userPin;

    public void UpdateUserLocation(MapControl map, double latitude, double longitude)
    {
        var location = new Location(latitude, longitude);

        if (userPin == null)
        {
            userPin = new Pin
            {
                Label = "Bạn đang ở đây",
                Location = location,
                Type = PinType.SavedPin
            };

            map.Pins.Add(userPin);
        }
        else
        {
            userPin.Location = location;
        }
    }

    // Hiển thị POI từ danh sách
    public void AddLocationPoints(MapControl map, List<LocationPoint> points)
    {
        if (points == null || points.Count == 0)
            return;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            foreach (var p in points)
            {
                var pin = new Pin
                {
                    Label = p.Name,
                    Address = p.Description,
                    Location = new Location(p.Latitude, p.Longitude),
                    Type = PinType.Place
                };

                map.Pins.Add(pin);
            }
        });
    }

    // Hiển thị POI lấy từ SQLite
    public async Task AddLocationPointsFromDbAsync(MapControl map, bool clearExisting = true)
    {
        try
        {
            // Ensure DB/tables exist
            await _sqlite.InitializeAsync();

            var points = await _sqlite.GetAllLocationPointsAsync();

            if (points == null || points.Count == 0)
                return;

            if (clearExisting)
            {
                MainThread.BeginInvokeOnMainThread(() => map.Pins.Clear());
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                foreach (var p in points)
                {
                    var pin = new Pin
                    {
                        Label = p.Name,
                        Address = p.Description,
                        Location = new Location(p.Latitude, p.Longitude),
                        Type = PinType.Place
                    };

                    map.Pins.Add(pin);
                }
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MapService] AddLocationPointsFromDbAsync error: {ex.Message}");
        }
    }

    // Xóa POI
    public void ClearPOI(MapControl map)
    {
        map.Pins.Clear();
    }
}