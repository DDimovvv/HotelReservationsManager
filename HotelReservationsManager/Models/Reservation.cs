using Hotel_Reservations_Manager.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Hotel_Reservations_Manager.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }
        [ForeignKey("Room")]
        public int RoomNumber { get; set; }
        public Room? Room { get; set; }
        [ForeignKey("Client")]
        public int ClientId { get; set; }
        public Client? Client { get; set; }
        public List<Client>? Clients { get; set; }
        [Required]
        public DateTime CheckInDate { get; set; }
        [Required]
        public DateTime CheckOutDate { get; set; }
        public bool BreakfastIncluded { get; set; }
        public bool AllInclusive { get; set; }
        [Required]
        public double FinalPrice { get; set; }
    }
}