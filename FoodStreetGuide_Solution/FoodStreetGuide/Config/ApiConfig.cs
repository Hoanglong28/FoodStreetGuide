using System.Net;
using System.Net.Sockets;

namespace doanC_.Config
{
    public static class ApiConfig
    {
        // ========== CẤU HÌNH CHUNG ==========
        private const int Port = 5225;                    // Port an toàn, tránh xung đột
        private const string ApiPath = "/api/LocationApi";

        // ========== CẤU HÌNH CHẾ ĐỘ ==========
        // Chỉ cần đổi dòng này: "NgrokMode" hoặc "LanMode" hoặc "LocalhostMode" hoặc "EmulatorMode"
        private const string ActiveMode = "NgrokMode";    // ← SỬA DÒNG NÀY TÙY THEO NHU CẦU

        // ========== CÁC CHẾ ĐỘ ==========

        // LAN Mode: Dùng khi cùng mạng WiFi (thay IP bằng IP thật của máy tính)
        private const string LanMode = "http://192.168.1.100:5225/api/LocationApi";

        // Ngrok Mode: Dùng khi demo từ xa (cập nhật URL mỗi khi chạy ngrok)
        private const string NgrokMode = "https://tapeless-nondivergently-eleni.ngrok-free.dev/api/LocationApi";

        // Localhost Mode: Dùng khi debug cùng máy
        private const string LocalhostMode = "http://localhost:5225/api/LocationApi";

        // Emulator Mode: Dùng khi chạy Android Emulator
        private const string EmulatorMode = "http://10.0.2.2:5225/api/LocationApi";

        // ========== HÀM LẤY URL ==========
        public static string GetBaseUrl()
        {
            return ActiveMode switch
            {
                "LanMode" => LanMode,
                "NgrokMode" => NgrokMode,
                "LocalhostMode" => LocalhostMode,
                "EmulatorMode" => EmulatorMode,
                _ => LocalhostMode
            };
        }

        // ========== TỰ ĐỘNG LẤY IP (DÙNG CHO LAN MODE ĐỘNG) ==========
        public static string GetDynamicLanUrl()
        {
            var ip = GetLocalIPAddress();
            return $"http://{ip}:{Port}{ApiPath}";
        }

        // ========== LẤY IP THẬT CỦA MÁY TÍNH ==========
        private static string GetLocalIPAddress()
        {
            try
            {
                var hostName = Dns.GetHostName();
                var hostEntry = Dns.GetHostEntry(hostName);

                foreach (var ip in hostEntry.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip))
                    {
                        return ip.ToString();
                    }
                }
                return "localhost";
            }
            catch
            {
                return "localhost";
            }
        }

        // ========== TIỆN ÍCH: KIỂM TRA URL HIỆN TẠI ==========
        public static void PrintCurrentUrl()
        {
            System.Diagnostics.Debug.WriteLine($"[ApiConfig] Current Mode: {ActiveMode}");
            System.Diagnostics.Debug.WriteLine($"[ApiConfig] Base URL: {GetBaseUrl()}");
        }
    }
}