using CardCatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CardCatalogService.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Card> Cards { get; set; }

        public DbSet<CardImage> CardImages { get; set; }

        public DbSet<CardReservation> CardReservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Card → CardImages ilişkisi
            modelBuilder.Entity<Card>()
                .HasMany(c => c.CardImages)
                .WithOne(i => i.Card)
                .HasForeignKey(i => i.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            // CARD Tablosu
            modelBuilder.Entity<Card>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.ExternalId).IsUnique(); // Dış API'den gelen ID benzersiz olsun

                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(e => e.Type)
                      .HasMaxLength(100);

                entity.Property(e => e.Description)
                      .HasMaxLength(8000); // Uzun açıklama

                entity.Property(e => e.Price)
                      .HasColumnType("decimal(10,2)");
            });

            // CARD IMAGE Tablosu
            modelBuilder.Entity<CardImage>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ExternalImageId)
                      .IsRequired();

                entity.Property(e => e.Url)
                      .HasMaxLength(500);

                entity.Property(e => e.UrlSmall)
                      .HasMaxLength(255);

                entity.Property(e => e.UrlCropped)
                      .HasMaxLength(255);
            });
        }
    }
}
