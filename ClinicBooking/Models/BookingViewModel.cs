using System.ComponentModel.DataAnnotations;

namespace ClinicBooking.Models
{
    public class BookingViewModel
    {
        public int DoctorId { get; set; }

        public string DoctorName { get; set; } = string.Empty;


        [Required(ErrorMessage = "Patient name is required.")]
        [StringLength(
            100,
            MinimumLength = 2,
            ErrorMessage = "Patient name must be between 2 and 100 characters.")]
        public string PatientName { get; set; } = string.Empty;

        public string? PhotoName { get; set; }

        [Required(ErrorMessage = "Patient phone is required.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(
            20,
            ErrorMessage = "Phone number cannot exceed 20 characters.")]
        public string PatientPhone { get; set; } = string.Empty;


        [Required(ErrorMessage = "Appointment date and time are required.")]
        public DateTime SlotDateTime { get; set; }


        [StringLength(
            500,
            ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string Notes { get; set; } = string.Empty;
    }
}