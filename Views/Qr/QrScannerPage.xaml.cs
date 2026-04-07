using System.Text.Json;
using doanC_.Models;
using doanC_.Services;
using doanC_.ViewModels;
using ZXing.Net.Maui;
using doanC_.Services.Geo;
using doanC_.Services.Localization;

namespace doanC_.Views;

public partial class QrScannerPage : ContentPage
{
    private bool _isScanning = true;
    private QrScannerViewModel _viewModel;

    public QrScannerPage()
    {
        InitializeComponent();
        _viewModel = new QrScannerViewModel();
        this.BindingContext = _viewModel;
    }

    /// <summary>
    /// Khi trang được hiển thị, cập nhật ngôn ngữ
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // 🆕 Cập nhật ngôn ngữ mỗi khi trang được hiển thị
        _viewModel.LoadLanguage();

        RequestCameraPermissionAndStartScanning();
    }

    private async void RequestCameraPermissionAndStartScanning()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

        if (status != PermissionStatus.Granted)
            status = await Permissions.RequestAsync<Permissions.Camera>();

        if (status != PermissionStatus.Granted)
        {
            await DisplayAlert(AppResources.GetString("Error"), AppResources.GetString("CameraPermissionDenied"), AppResources.GetString("OK"));
            return;
        }

        cameraView.IsDetecting = true;
    }

    // 🔥 EVENT QUÉT QR (QUAN TRỌNG NHẤT)
    private void OnBarcodeDetected(object sender, BarcodeDetectionEventArgs e)
    {
        if (!_isScanning) return;

        var result = e.Results.FirstOrDefault();
        if (result == null) return;

        _isScanning = false;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            cameraView.IsDetecting = false;

            await HandleQr(result.Value);

            // Cho phép quét lại
            _isScanning = true;
            cameraView.IsDetecting = true;
        });
    }

    // 🔥 XỬ LÝ QR
    private async Task HandleQr(string qrText)
    {
        try
        {
            // Parse JSON → LocationPoint
            var point = JsonSerializer.Deserialize<LocationPoint>(qrText);

            if (point != null)
            {
                await DisplayAlert(AppResources.GetString("OK"), $"{AppResources.GetString("AddedToFavorite")}: {point.Name}", AppResources.GetString("OK"));

                GeoFenceService.Instance.AddPoint(point);
            }
            else
            {
                await DisplayAlert("QR", qrText, AppResources.GetString("OK"));
            }
        }
        catch
        {
            await DisplayAlert(AppResources.GetString("Error"), AppResources.GetString("InvalidQrFormat"), AppResources.GetString("OK"));
        }
    }

    // 🔥 NHẬP TAY (fallback)
    private async void OnManualInputClicked(object sender, EventArgs e)
    {
        string input = await DisplayPromptAsync(AppResources.GetString("QrManualInput"), AppResources.GetString("PasteQrContent"), AppResources.GetString("OK"), AppResources.GetString("Cancel"));

        if (!string.IsNullOrEmpty(input))
        {
            await HandleQr(input);
        }
    }
}