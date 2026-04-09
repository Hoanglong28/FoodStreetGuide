using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using SkiaSharp.Views.Maui.Controls.Hosting;
using ZXing.Net.Maui.Controls;
using doanC_.Services.Data;
using doanC_.Services.Localization;
using doanC_.Services.Audio;
using doanC_.Services;

namespace doanC_
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiMaps()
                .UseSkiaSharp()
                .UseBarcodeReader()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("fa-solid-900.ttf", "FontAwesome");
                })
                .Services
                .AddSingleton<SQLiteService>()
                .AddSingleton<SeedDataService>()
                .AddSingleton<LibreTranslateService>()
                .AddSingleton<TTSService>()
                .AddSingleton<LocationService>() 
                .AddSingleton<GeofenceService>(); // ✅ Thêm GeofenceService

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}