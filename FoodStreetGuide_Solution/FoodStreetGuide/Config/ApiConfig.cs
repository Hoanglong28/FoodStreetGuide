namespace doanC_.Config
{
    public static class ApiConfig
    {
        // Thay đổi IP này thành IP máy tính của bạn
        // Lấy IP: mở CMD -> ipconfig -> IPv4 Address
        private const string LocalIp = "192.168.1.100";

        // Port của Admin Web (xem trong launchSettings.json)
        private const string HttpsPort = "5001";
        private const string HttpPort = "5000";

        public static string BaseUrl => $"https://{LocalIp}:{HttpsPort}/api/LocationPointsApi";

        // Dùng HTTP nếu gặp lỗi SSL
        public static string BaseUrlHttp => $"http://{LocalIp}:{HttpPort}/api/LocationPointsApi";

        // Cho debug trên Windows Machine
        public static string LocalhostUrl => "https://localhost:5001/api/LocationPointsApi";
    }
}