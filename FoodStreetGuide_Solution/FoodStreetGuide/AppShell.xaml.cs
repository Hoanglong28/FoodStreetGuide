using doanC_.Views;
using doanC_.Views.Language;

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
    }
}
