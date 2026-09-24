using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicBooking.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [ForeignKey(nameof(DoctorId))]
        public Doctor? Doctor { get; set; }

        [Required]
        [StringLength(100)]
        public string PatientName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string PatientPhone { get; set; } = string.Empty;

        [Required]
        public DateTime SlotDateTime { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}