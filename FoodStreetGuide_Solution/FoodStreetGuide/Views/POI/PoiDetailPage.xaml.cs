using System.Diagnostics;
using System.Linq;
using doanC_.Helpers;
using doanC_.Models;
using doanC_.Services;
using doanC_.Services.Api;  // ← THÊM DÒNG NÀY
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Media;

namespace doanC_.Views;

[QueryProperty(nameof(PoiId), "poiId")]
public partial class PoiDetailPage : ContentPage
{
    private ApiService _apiService;  // ← ĐỔI từ SQLiteService sang ApiService
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

        _apiService = new ApiService();  // ← DÙNG API SERVICE
        _locationService = new LocationService();

        AudioLanguagePicker.SelectedIndex = 0;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await GetCurrentLocationAndCalculateDistance();
    }

    private async Task LoadPoiDetailsAsync(int poiId)
    {
        try
        {
            if (poiId == 0)
                return;

            // GỌI API THAY VÌ SQLITE
            _currentPoi = await _apiService.GetLocationPointByIdAsync(poiId);

            if (_currentPoi == null)
            {
                await DisplayAlert("Lỗi", "Không tìm thấy địa điểm", "OK");
                await GoBackAsync();
                return;
            }

            UpdatePoiUI();

            _userLocation = await _locationService.GetCurrentLocationAsync();

            if (_userLocation != null)
                UpdateDistance();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[PoiDetailPage] Error: {ex.Message}");
            await DisplayAlert("Lỗi", $"Không thể tải chi tiết: {ex.Message}", "OK");
        }
    }

    private void UpdatePoiUI()
    {
        if (_currentPoi == null)
            return;

        Title = _currentPoi.Name;

        MainImage.Source = _currentPoi.Image ?? "poi_default.png";
        PoiNameLabel.Text = _currentPoi.Name;

        RatingLabel.Text = $"★ {_currentPoi.Rating} ({_currentPoi.ReviewCount})";

        CategoryLabel.Text = (_currentPoi.Category ?? "").ToUpper();
        AddressLabel.Text = _currentPoi.Address;
        DescriptionLabel.Text = _currentPoi.Description;

        OpeningHoursLabel.Text = string.IsNullOrEmpty(_currentPoi.OpeningHours)
            ? "⏰ 8:00 – 18:00"
            : $"⏰ {_currentPoi.OpeningHours}";

        PriceLabel.Text = GetPriceRangeDisplay(_currentPoi.PriceRange);
    }

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

        double distance = CalculateDistance(
            _userLocation.Latitude,
            _userLocation.Longitude,
            _currentPoi.Latitude,
            _currentPoi.Longitude);

        Debug.WriteLine($"Distance: {(int)distance}m");

        // Hiển thị khoảng cách lên UI nếu có label
        // DistanceLabel.Text = $"Cách bạn {(int)distance}m";
    }

    private async Task GetCurrentLocationAndCalculateDistance()
    {
        try
        {
            _userLocation = await _locationService.GetCurrentLocationAsync();
            if (_userLocation != null)
                UpdateDistance();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Location error: {ex.Message}");
        }
    }

    private async Task GoBackAsync()
    {
        if (Shell.Current.Navigation.NavigationStack.Count > 1)
            await Shell.Current.GoToAsync("..");
        else
            await Shell.Current.GoToAsync("///PoiListPage");
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await GoBackAsync();
    }

    private async void OnFavoriteClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Yêu thích", "Đã thêm vào yêu thích", "OK");
    }

    private async void OnShareClicked(object sender, EventArgs e)
    {
        if (_currentPoi == null)
            return;

        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Text = $"Xem {_currentPoi.Name} tại {_currentPoi.Address}",
            Title = _currentPoi.Name
        });
    }

    private async void OnOpenStatusClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Thông tin", "Đang mở cửa", "OK");
    }

    private void OnPlayAudioClicked(object sender, EventArgs e)
    {
        _isAudioPlayerVisible = !_isAudioPlayerVisible;
        AudioPlayerFrame.IsVisible = _isAudioPlayerVisible;
    }

    private async void OnGetDirectionsClicked(object sender, EventArgs e)
    {
        if (_currentPoi == null)
            return;

        var url = $"https://www.google.com/maps/search/?api=1&query={_currentPoi.Latitude},{_currentPoi.Longitude}";
        await Launcher.Default.OpenAsync(url);
    }

    private void OnLanguageChanged(object sender, EventArgs e)
    {
        if (AudioLanguagePicker.SelectedIndex >= 0)
        {
            string lang = AudioLanguagePicker.Items[AudioLanguagePicker.SelectedIndex];
            Debug.WriteLine($"Selected language: {lang}");
        }
    }

    private async Task<Locale> GetSelectedLocaleAsync()
    {
        var locales = await TextToSpeech.Default.GetLocalesAsync();

        string selected = AudioLanguagePicker.SelectedItem?.ToString() ?? "Tiếng Việt";

        if (selected == "English")
            return locales.FirstOrDefault(l => l.Language.StartsWith("en"));

        if (selected == "中文")
            return locales.FirstOrDefault(l => l.Language.StartsWith("zh"));

        if (selected == "日本語")
            return locales.FirstOrDefault(l => l.Language.StartsWith("ja"));

        if (selected == "한국어")
            return locales.FirstOrDefault(l => l.Language.StartsWith("ko"));

        return locales.FirstOrDefault(l => l.Language.StartsWith("vi"));
    }

    private async void OnPlayPauseClicked(object sender, EventArgs e)
    {
        try
        {
            if (_currentPoi == null)
                return;

            if (PlayPauseButton.Text == "▶ Phát")
            {
                PlayPauseButton.Text = "⏸ Dừng";

                string text = $"{_currentPoi.Name}. {_currentPoi.Description}";

                var locale = await GetSelectedLocaleAsync();

                await TextToSpeech.Default.SpeakAsync(
                    text,
                    new SpeechOptions
                    {
                        Locale = locale,
                        Pitch = 1f,
                        Volume = 1f
                    });

                PlayPauseButton.Text = "▶ Phát";
            }
            else
            {
                PlayPauseButton.Text = "▶ Phát";
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"TTS Error: {ex.Message}");
        }
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double R = 6371000;

        double dLat = ToRad(lat2 - lat1);
        double dLon = ToRad(lon2 - lon1);

        double a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(ToRad(lat1)) *
            Math.Cos(ToRad(lat2)) *
            Math.Sin(dLon / 2) *
            Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }

    private double ToRad(double val)
    {
        return val * Math.PI / 180;
    }
}