using Microsoft.EntityFrameworkCore;

namespace doanC_Admin.Models
{
    public class FoodStreetGuideDBContext : DbContext
    {
        public FoodStreetGuideDBContext(DbContextOptions<FoodStreetGuideDBContext> options)
            : base(options)
        {
        }

        public DbSet<LocationPoint> LocationPoints { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình tên bảng và khóa chính
            modelBuilder.Entity<LocationPoint>(entity =>
            {
                entity.ToTable("LocationPoints");
                entity.HasKey(e => e.PointId);  // Map PointId là khóa chính
                entity.Property(e => e.PointId).HasColumnName("PointId");
                entity.Property(e => e.Name).HasColumnName("Name");
                entity.Property(e => e.Description).HasColumnName("Description");
                entity.Property(e => e.Latitude).HasColumnName("Latitude");
                entity.Property(e => e.Longitude).HasColumnName("Longitude");
                entity.Property(e => e.Radius).HasColumnName("Radius");
                entity.Property(e => e.AudioFile).HasColumnName("AudioFile");
                entity.Property(e => e.Language).HasColumnName("Language");
                entity.Property(e => e.Address).HasColumnName("Address");
                entity.Property(e => e.Category).HasColumnName("Category");
                entity.Property(e => e.Image).HasColumnName("Image");
                entity.Property(e => e.Rating).HasColumnName("Rating");
                entity.Property(e => e.ReviewCount).HasColumnName("ReviewCount");
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt");
                entity.Property(e => e.OpeningHours).HasColumnName("OpeningHours");
                entity.Property(e => e.PriceRange).HasColumnName("PriceRange");
            });
        }
    }
}