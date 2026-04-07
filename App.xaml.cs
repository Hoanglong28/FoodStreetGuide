using doanC_.Services.Data;
using doanC_.Helpers;
using doanC_.Models;
using doanC_.Services.Localization; 
using SQLite;
using System.Diagnostics;
using Microsoft.Maui.Storage;

namespace doanC_
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // 🆕 Tải ngôn ngữ đã lưu
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
                var databaseService = ServiceHelper.GetService<SQLiteService>();
                var seedService = ServiceHelper.GetService<SeedDataService>();
                var translationService = ServiceHelper.GetService<LibreTranslateService>();

                if (databaseService != null && seedService != null)
                {
                    await databaseService.InitializeAsync();
                    Debug.WriteLine("[App] Database initialized successfully");

                    translationService?.Initialize();
                    Debug.WriteLine("[App] ✅ LibreTranslate service initialized");

                    await seedService.SeedAsync();

                    var users = await databaseService.GetAllUsersAsync();
                    var locations = await databaseService.GetAllLocationPointsAsync();
                    var database = databaseService.Database;
                    var dishes = await database.Table<Dish>().ToListAsync();
                    var reviews = await database.Table<Review>().ToListAsync();

                    Debug.WriteLine($"\n📊 DATABASE CONTENTS:\nUsers: {users.Count}, LocationPoints: {locations.Count}, Dishes: {dishes.Count}, Reviews: {reviews.Count}\n");
                    
                    foreach (var user in users)
                        Debug.WriteLine($"👤 User: {user.Id} - {user.Username} ({user.Email})");
                    
                    foreach (var loc in locations)
                        Debug.WriteLine($"🗺️ Location: {loc.Id} - {loc.Name} at ({loc.Latitude}, {loc.Longitude})");
                    
                    foreach (var dish in dishes)
                        Debug.WriteLine($"🍽️ Dish: {dish.Id} - {dish.Name} (${dish.Price}) from LocationId {dish.LocationPointId}");
                    
                    foreach (var review in reviews)
                        Debug.WriteLine($"⭐ Review: {review.Id} - Rating {review.Rating} by UserId {review.UserId} - \"{review.Comment}\"");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[App] Error: {ex.Message}");
                Debug.WriteLine($"[App] Stack trace: {ex.StackTrace}");
            }
        }
    }
}