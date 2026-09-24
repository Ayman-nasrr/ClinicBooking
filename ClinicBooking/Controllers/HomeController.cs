using ClinicBooking.Models;
using ClinicBooking.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ClinicBooking.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IDoctorRepository doctorRepo;
        private readonly IAppointmentRepository appointmentRepo;

        public HomeController(
            ILogger<HomeController> logger,
            IDoctorRepository doctorRepo,
            IAppointmentRepository appointmentRepo)
        {
            _logger = logger;
            this.doctorRepo = doctorRepo;
            this.appointmentRepo = appointmentRepo;
        }


        public IActionResult Index()
        {
            var doctors =
                doctorRepo.GetAll();

            var specialties =
                doctors
                    .Select(d => d.Specialty)
                    .Where(s =>
                        !string.IsNullOrWhiteSpace(s))
                    .Distinct()
                    .ToList();


            // Get appointment count from database
            var appointmentsCount =
                appointmentRepo.GetAll().Count;


            ViewBag.DoctorsCount =
                doctors.Count;

            ViewBag.SpecialtiesCount =
                specialties.Count;

            ViewBag.AppointmentsCount =
                appointmentsCount;


            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId =
                        Activity.Current?.Id
                        ?? HttpContext.TraceIdentifier
                });
        }
    }
}