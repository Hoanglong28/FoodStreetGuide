using System.Net;
using System.Net.Sockets;

namespace doanC_.Config
{
    public static class ApiConfig
    {
        // Port Admin Web đang chạy (hiện tại là 5220)
        private const string Port = "5220";

        // Đường dẫn API
        private const string ApiPath = "/api/LocationApi";

        // Lấy BaseUrl tự động dựa trên IP máy tính
        public static class ApiMode
        {
            // Chế độ LAN (phát triển - thay IP bằng IP thật của máy bạn)
            private const string LanMode = "http://192.168.1.100:5220/api/LocationApi";

            // Chế độ Ngrok (demo - thay URL bằng URL ngrok của bạn)
            private const string NgrokMode = "https://abc1def2gh3.ngrok-free.app/api/LocationApi";

            // CHỌN CHẾ ĐỘ TRƯỚC KHI BUILD
            public static string GetBaseUrl()
            {
                // Đổi thành NgrokMode khi demo, LanMode khi phát triển
                return "https://tapeless-nondivergently-eleni.ngrok-free.dev/api/LocationApi";
            }
        }

        // URL cho localhost (debug cùng máy)
        public static string LocalhostUrl => $"http://localhost:{Port}{ApiPath}";

        // URL cho Android Emulator
        public static string EmulatorUrl => $"http://10.0.2.2:{Port}{ApiPath}";

        // Hàm tự động lấy IP của máy tính đang chạy
        private static string GetLocalIPAddress()
        {
            try
            {
                // Lấy tên máy tính
                var hostName = Dns.GetHostName();
                var hostEntry = Dns.GetHostEntry(hostName);

                // Tìm địa chỉ IPv4 đầu tiên (không phải loopback)
                foreach (var ip in hostEntry.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork &&
                        !IPAddress.IsLoopback(ip))
                    {
                        return ip.ToString();
                    }
                }

                // Nếu không tìm thấy, dùng localhost
                return "localhost";
            }
            catch
            {
                return "localhost";
            }
        }
    }
}