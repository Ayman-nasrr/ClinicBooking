using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
namespace ClinicBooking.Models
{
    public class Doctor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the doctor's full name.")]
        [StringLength(50,
    MinimumLength = 3,
    ErrorMessage = "Doctor name must be between 3 and 50 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the doctor's phone number.")]
        [RegularExpression(
    @"^01[0125][0-9]{8}$",
    ErrorMessage = "Enter a valid Egyptian mobile number."
)]
        public string Phone { get; set; } = string.Empty;

        [Remote(
    action: "CheckLicenceNumber",
    controller: "Doctors",
    AdditionalFields = nameof(Id),
    ErrorMessage = "This licence number is already registered."
)]
        public string LicenceNumber { get; set; } = string.Empty;

        public string? PhotoName { get; set; }

        public string Specialty { get; set; } = string.Empty;

        public string Bio { get; set; } = string.Empty;

        [Range(0, 60,
    ErrorMessage = "Years of experience must be between 0 and 60.")]
        public int YearsOfExperience { get; set; }
        public bool IsAcceptingPatients { get; set; } = true;
    }
}