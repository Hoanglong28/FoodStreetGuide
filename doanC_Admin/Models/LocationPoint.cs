namespace doanC_Admin.Models
{
    public class LocationPoint
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double Radius { get; set; }

        public string AudioFile { get; set; }

        public string Language { get; set; }

        // ===== THÊM CÁC FIELD MỚI CHO KHỚP VỚI MAUI =====
        public string Address { get; set; }

        public string Category { get; set; }

        public string Image { get; set; }

        public double Rating { get; set; } = 0;

        public int ReviewCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public string OpeningHours { get; set; }

        public string PriceRange { get; set; }
    }
}