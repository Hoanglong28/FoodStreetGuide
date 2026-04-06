using System.Diagnostics;
using doanC_.Helpers;
using doanC_.Models;
using doanC_.Services.Data;
using doanC_.Services;
using Microsoft.Maui.Devices.Sensors;

namespace doanC_.Views;

public partial class PoiListPage : ContentPage
{
    private SQLiteService _sqliteService;
    private LocationService _locationService;
    private List<LocationPoint> _allLocationPoints;
    private Location _currentLocation;

    public PoiListPage()
    {
        InitializeComponent();
        _sqliteService = ServiceHelper.GetService<SQLiteService>();
        _locationService = new LocationService();
        LoadDataFromDatabase();
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

    private async void LoadDataFromDatabase()
    {
        try
        {
            _allLocationPoints = await _sqliteService.GetAllLocationPointsAsync();

            var poiItems = _allLocationPoints.Select(location => new PoiItem
            {
                Id = location.Id,
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

            Debug.WriteLine($"[PoiListPage] Loaded {poiItems.Count} POI items from database");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[PoiListPage] Error loading data: {ex.Message}");
            await DisplayAlert("Lỗi", $"Không thể tải dữ liệu: {ex.Message}", "OK");
        }
    }

    private void RefreshDistances()
    {
        if (_currentLocation == null || _allLocationPoints == null)
            return;

        var updatedItems = _allLocationPoints.Select(location => new PoiItem
        {
            Id = location.Id,
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
                           l.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .Select(location => new PoiItem
                {
                    Id = location.Id,
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
                Debug.WriteLine($"[PoiListPage] Selected POI: {selectedPoi.Name} (ID: {selectedPoi.Id})");
                await Shell.Current.GoToAsync($"///poidetailpage?poiId={selectedPoi.Id}");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[PoiListPage] Error: {ex.Message}");
        }
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
    public int Id { get; set; }
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
