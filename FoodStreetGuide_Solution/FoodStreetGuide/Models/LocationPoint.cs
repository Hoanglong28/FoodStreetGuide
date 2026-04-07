using SQLite;

namespace doanC_.Models
{
    [Table("LocationPoint")]
    public class LocationPoint
    {
        [PrimaryKey, AutoIncrement]
        public int PointId { get; set; } 

        [NotNull]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [NotNull]
        public string Address { get; set; } = string.Empty;

        [NotNull]
        public double Latitude { get; set; }

        [NotNull]
        public double Longitude { get; set; }

        public string Category { get; set; } = string.Empty;

        public string Image { get; set; } = string.Empty;

        public double Rating { get; set; } = 0;

        public int ReviewCount { get; set; } = 0;

        [NotNull]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public string OpeningHours { get; set; } = string.Empty;

        public string PriceRange { get; set; } = string.Empty;

        // Constructor cho compatibility
        public LocationPoint()
        {
        }

        public LocationPoint(string name, string description, double latitude, double longitude)
        {
            Name = name;
            Description = description;
            Latitude = latitude;
            Longitude = longitude;
            Address = "";
        }
    }
}