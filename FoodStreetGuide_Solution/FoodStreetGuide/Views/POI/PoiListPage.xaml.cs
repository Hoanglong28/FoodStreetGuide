using System.Diagnostics;
using doanC_.Helpers;
using doanC_.Models;
using doanC_.Services.Api;
using doanC_.Services;
using Microsoft.Maui.Devices.Sensors;

namespace doanC_.Views;

public partial class PoiListPage : ContentPage
{
    private ApiService _apiService;
    private LocationService _locationService;
    private List<LocationPoint> _allLocationPoints;
    private Location _currentLocation;
    private string _currentCategory = "Tất cả";

    public PoiListPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
        _locationService = new LocationService();

        LoadDataFromApi();
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
                RefreshPoiList();
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

    private async void LoadDataFromApi()
    {
        try
        {
            loadingIndicator.IsVisible = true;

            _allLocationPoints = await _apiService.GetLocationPointsAsync();

            if (_allLocationPoints != null && _allLocationPoints.Any())
            {
                RefreshPoiList();
                Debug.WriteLine($"[PoiListPage] Loaded {_allLocationPoints.Count} POI items from API");
            }
            else
            {
                Debug.WriteLine("[PoiListPage] No data from API");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[PoiListPage] Error loading data from API: {ex.Message}");
            await DisplayAlert("Lỗi", $"Không thể kết nối đến server: {ex.Message}", "OK");
        }
        finally
        {
            loadingIndicator.IsVisible = false;
        }
    }

    private void RefreshPoiList()
    {
        if (_allLocationPoints == null) return;

        // Lọc theo category
        var source = _currentCategory == "Tất cả"
            ? _allLocationPoints
            : _allLocationPoints.Where(p => p.Category == _currentCategory).ToList();

        // Tính khoảng cách và sắp xếp
        var poiItems = source.Select(location => new PoiItem
        {
            PointId = location.PointId,
            Name = location.Name,
            Description = location.Description ?? "",
            Distance = _currentLocation != null
                ? (int)CalculateDistance(_currentLocation.Latitude, _currentLocation.Longitude, location.Latitude, location.Longitude)
                : 0,
            ImageUrl = location.Image ?? "poi_default.png",
            Category = location.Category ?? "",
            Rating = location.Rating,
            ReviewCount = location.ReviewCount,
            Latitude = location.Latitude,
            Longitude = location.Longitude
        })
        .OrderBy(p => p.Distance)
        .ToList();

        PoiCollection.ItemsSource = poiItems;
    }

    private void OnCategoryTapped(object sender, TappedEventArgs e)
    {
        var frame = sender as Frame;
        var label = frame?.Content as Label;
        if (label != null)
        {
            _currentCategory = label.Text;
            UpdateCategoryUI();
            RefreshPoiList();
        }
    }

    private void UpdateCategoryUI()
    {
        // Reset all category frames
        var categories = new Dictionary<Frame, Label>
        {
            { CategoryAll, CategoryAll.Content as Label },
            { CategoryFood, CategoryFood.Content as Label },
            { CategoryPlace, CategoryPlace.Content as Label },
            { CategoryHistory, CategoryHistory.Content as Label }
        };

        foreach (var cat in categories)
        {
            var isActive = (cat.Value?.Text == _currentCategory);
            cat.Key.BackgroundColor = isActive ? Color.FromArgb("#C85A3F") : Color.FromArgb("#E0D5CC");
            if (cat.Value != null)
            {
                cat.Value.TextColor = isActive ? Colors.White : Color.FromArgb("#2C1810");
            }
        }
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        SearchData(e.NewTextValue);
    }

    private void SearchData(string searchText)
    {
        if (_allLocationPoints == null) return;

        if (string.IsNullOrWhiteSpace(searchText))
        {
            RefreshPoiList();
        }
        else
        {
            var source = _allLocationPoints
                .Where(l => l.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                           (l.Description != null && l.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            var filteredItems = source.Select(location => new PoiItem
            {
                PointId = location.PointId,
                Name = location.Name,
                Description = location.Description ?? "",
                Distance = _currentLocation != null
                    ? (int)CalculateDistance(_currentLocation.Latitude, _currentLocation.Longitude, location.Latitude, location.Longitude)
                    : 0,
                ImageUrl = location.Image ?? "poi_default.png",
                Category = location.Category ?? "",
                Rating = location.Rating,
                ReviewCount = location.ReviewCount,
                Latitude = location.Latitude,
                Longitude = location.Longitude
            })
            .OrderBy(p => p.Distance)
            .ToList();

            PoiCollection.ItemsSource = filteredItems;
        }
    }

    private async void OnPoiTapped(object sender, TappedEventArgs e)
    {
        try
        {
            var frame = sender as Frame;
            if (frame?.BindingContext is PoiItem selectedPoi)
            {
                await Shell.Current.GoToAsync($"///poidetailpage?poiId={selectedPoi.PointId}");
                Debug.WriteLine($"[PoiListPage] Selected POI: {selectedPoi.Name} (ID: {selectedPoi.PointId})");
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
    public int PointId { get; set; }
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