using doanC_.Helpers;
using doanC_.Models;
using doanC_.Services.Localization;
using System.Diagnostics;
using Microsoft.Maui.Storage;

namespace doanC_
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Tải ngôn ngữ đã lưu
            var appLanguage = Preferences.Get("AppLanguage", null);
            if (!string.IsNullOrEmpty(appLanguage))
            {
                AppResources.SetLanguage(appLanguage);
                Debug.WriteLine($"[App] 🌐 Loaded saved language: {appLanguage}");
            }

            // Kiểm tra nếu đã chọn ngôn ngữ, hiển thị AppShell; nếu chưa, hiển thị LanguageSelectionPage
            if (string.IsNullOrEmpty(appLanguage))
            {
                MainPage = new NavigationPage(new Views.Language.LanguageSelectionPage());
            }
            else
            {
                MainPage = new AppShell();
            }
        }

        protected override async void OnStart()
        {
            base.OnStart();

            try
            {
                // ✅ KHÔNG CẦN SQLite NỮA - DỮ LIỆU LẤY TỪ API
                var translationService = ServiceHelper.GetService<LibreTranslateService>();

                if (translationService != null)
                {
                    translationService.Initialize();
                    Debug.WriteLine("[App] ✅ LibreTranslate service initialized");
                }

                Debug.WriteLine("[App] ✅ App started - Data will be loaded from API");
                Debug.WriteLine("[App] 💡 Make sure Admin Web is running at: http://localhost:5225");
                Debug.WriteLine("[App] 💡 Or Ngrok URL configured in ApiConfig.cs");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[App] Error: {ex.Message}");
                Debug.WriteLine($"[App] Stack trace: {ex.StackTrace}");
            }
        }
    }
}