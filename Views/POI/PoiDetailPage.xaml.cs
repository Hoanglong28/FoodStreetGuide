using System.Diagnostics;
using doanC_.Helpers;
using doanC_.Models;
using doanC_.Services.Data;
using doanC_.Services;
using Microsoft.Maui.Devices.Sensors;

namespace doanC_.Views;

[QueryProperty(nameof(PoiId), "poiId")]
public partial class PoiDetailPage : ContentPage
{
    private SQLiteService _sqliteService;
    private LocationService _locationService;
    private LocationPoint _currentPoi;
    private Location _userLocation;
    private bool _isAudioPlayerVisible = false;

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

    public PoiDetailPage()
    {
        InitializeComponent();
        _sqliteService = ServiceHelper.GetService<SQLiteService>();
        _locationService = new LocationService();
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
                await DisplayAlert("Lỗi", "Không tìm thấy thông tin địa điểm", "OK");
                await GoBackAsync();
                return;
            }

            UpdatePoiUI();

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
            await DisplayAlert("Lỗi", $"Không thể tải dữ liệu: {ex.Message}", "OK");
            await GoBackAsync();
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
            
            CategoryLabel.Text = (_currentPoi.Category ?? "CHƯA PHÂN LOẠI").ToUpper();
            AddressLabel.Text = _currentPoi.Address ?? "Không rõ địa chỉ";
            DescriptionLabel.Text = _currentPoi.Description ?? "Không có mô tả";
            
            OpeningHoursLabel.Text = string.IsNullOrEmpty(_currentPoi.OpeningHours) 
                ? "⏰ 8:00 – 18:00" 
                : $"⏰ {_currentPoi.OpeningHours}";

            // ✅ Cập nhật giá dựa trên PriceRange
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
        return "Liên hệ để biết giá";

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
            // Cách 1: Dùng relative navigation
            if (Shell.Current.Navigation.NavigationStack.Count > 1)
            {
                Debug.WriteLine("[PoiDetailPage] Using relative back navigation");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                // Cách 2: Nếu stack trống, quay về PoiListPage
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
            await DisplayAlert("Yêu thích", "Đã thêm vào yêu thích", "OK");
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
                    Text = $"Xem {_currentPoi.Name} tại {_currentPoi.Address}",
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
        await DisplayAlert("Thông tin", "Quán đang mở cửa", "OK");
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
            await DisplayAlert("Lỗi", "Không thể mở bản đồ", "OK");
        }
    }

    private void OnLanguageChanged(object sender, EventArgs e)
    {
        if (AudioLanguagePicker.SelectedIndex >= 0)
        {
            var selectedLanguage = AudioLanguagePicker.Items[AudioLanguagePicker.SelectedIndex];
            Debug.WriteLine($"[PoiDetailPage] Selected language: {selectedLanguage}");
        }
    }

    private void OnPlayPauseClicked(object sender, EventArgs e)
    {
        try
        {
            PlayPauseButton.Text = PlayPauseButton.Text == "▶ Phát" ? "⏸ Dừng" : "▶ Phát";
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[PoiDetailPage] Error: {ex.Message}");
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