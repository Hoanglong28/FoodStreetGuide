using doanC_.Views;
using doanC_.Views.Language;
using doanC_.Services.Localization;
using System.Diagnostics;

namespace doanC_
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            this.InitializeComponent();

            // Đăng ký routes cho navigation
            Routing.RegisterRoute("MapPage", typeof(MapPage));
            Routing.RegisterRoute("PoiListPage", typeof(PoiListPage));
            Routing.RegisterRoute("PoiDetailPage", typeof(PoiDetailPage));
            Routing.RegisterRoute("QrScannerPage", typeof(QrScannerPage));
            Routing.RegisterRoute("SettingsPage", typeof(SettingsPage));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            // Delay một chút để đảm bảo mọi thứ khởi tạo xong
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(100);
                UpdateTabBarTitles();
            });
        }

        private void UpdateTabBarTitles()
        {
            try
            {
                var currentLang = AppResources.GetCurrentLanguage();
                Debug.WriteLine($"[AppShell] 🌐 Current language: {currentLang}");

                // TabBar là Items[0] nếu nó là nhóm đầu tiên
                if (this.Items?.Count > 0)
                {
                    var firstItem = this.Items[0];
                    Debug.WriteLine($"[AppShell] First item type: {firstItem.GetType().Name}");

                    // Nếu đó là TabBar, lấy children của nó
                    if (firstItem is TabBar tabBar)
                    {
                        Debug.WriteLine($"[AppShell] TabBar found with {tabBar.Items.Count} items");

                        for (int i = 0; i < tabBar.Items.Count && i < 4; i++)
                        {
                            if (tabBar.Items[i] is ShellSection section)
                            {
                                string[] keys = { "TabMap", "TabPoi", "TabQr", "TabSettings" };
                                if (i < keys.Length)
                                {
                                    section.Title = AppResources.GetString(keys[i]);
                                    Debug.WriteLine($"[AppShell] Set item {i} title to: {section.Title}");
                                }
                            }
                        }
                    }
                }

                Debug.WriteLine("[AppShell] ✅ UpdateTabBarTitles completed");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AppShell] ⚠️ Error: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
