using doanC_.Services.Data;
using doanC_.Helpers;
using doanC_.Models;
using SQLite;
using System.Diagnostics;

namespace doanC_
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Kiểm tra nếu đã chọn ngôn ngữ, hiển thị AppShell; nếu chưa, hiển thị LanguageSelectionPage
            var appLanguage = Microsoft.Maui.Storage.Preferences.Get("AppLanguage", null);
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
                var databaseService = ServiceHelper.GetService<SQLiteService>();

                if (databaseService != null)
                {
                    await databaseService.InitializeAsync();
                    Debug.WriteLine("[App] Database initialized successfully");

                    // ❌ TẮT SEED - không cần nữa vì dữ liệu từ API
                    // var seedService = ServiceHelper.GetService<SeedDataService>();
                    // await seedService.SeedAsync();

                    // Chỉ hiển thị thông báo đã sẵn sàng
                    Debug.WriteLine("[App] Ready to load data from API");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[App] Error: {ex.Message}");
            }
        }
    }
}