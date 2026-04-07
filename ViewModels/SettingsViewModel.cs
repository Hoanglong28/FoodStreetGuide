using doanC_.Services.Localization;

namespace doanC_.ViewModels
{
    public class SettingsViewModel : ILanguageRefresh
    {
        public string Settings                => AppResources.GetString("Settings");
        public string LanguageSettingsSection => AppResources.GetString("LanguageSettingsSection");
        public string LanguageLabel           => AppResources.GetString("LanguageLabel");
        public string VoiceTTSLabel           => AppResources.GetString("VoiceTTSLabel");
        public string GpsGeofenceSection      => AppResources.GetString("GpsGeofenceSection");
        public string RadiusActivationLabel   => AppResources.GetString("RadiusActivationLabel");
        public string BackgroundTrackingLabel => AppResources.GetString("BackgroundTrackingLabel");
        public string BatterySaveLabel        => AppResources.GetString("BatterySaveLabel");
        public string OfflineContentSection   => AppResources.GetString("OfflineContentSection");
        public string DownloadOfflineLabel    => AppResources.GetString("DownloadOfflineLabel");
        public string OfflinePackageInfo      => AppResources.GetString("OfflinePackageInfo");

        public SettingsViewModel()
        {
            LanguageChangeManager.Register(this);
        }

        public void RefreshLanguage()
        {
        }
    }
}
