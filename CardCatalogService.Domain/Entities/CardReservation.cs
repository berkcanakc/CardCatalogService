using System;

namespace CardCatalogService.Domain.Entities
{
    public class CardReservation
    {
        public int Id { get; set; }  // Projeye uygun olarak int kullandık

        public int CardId { get; set; }      // Hangi karta ait
        public int UserId { get; set; }     // Hangi kullanıcıya ait (auth sistemine göre Guid tercih ettik)
        public int CartId { get; set; }     // Hangi sepetten geldi
        public int Quantity { get; set; }    // Kaç adet rezerve edildi
        public DateTime ReservedAt { get; set; } = DateTime.UtcNow;

        public bool IsConfirmed { get; set; } = false; // Siparişe dönüşüp satıldı mı?
        public bool IsReleased { get; set; } = false;  // Süresi dolup iptal edildi mi?

        public bool IsActive => !IsConfirmed && !IsReleased;

        public Card Card { get; set; }  // Navigasyon
    }
}
