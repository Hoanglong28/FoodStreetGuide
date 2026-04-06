using Android.App;
using Android.Runtime;

namespace doanC_
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
            // Cho phép HTTP cho Android 9+
            var builder = new Android.Net.ConnectivityManager.NetworkCallback();
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
