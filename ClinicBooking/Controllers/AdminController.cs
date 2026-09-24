using Microsoft.AspNetCore.Mvc;
using ClinicBooking.Models;
using ClinicBooking.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace ClinicBooking.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IDoctorRepository doctorRepo;

        public AdminController(
            IDoctorRepository doctorRepo)
        {
            this.doctorRepo = doctorRepo;
        }

        public IActionResult Doctors()
        {
            var doctors = doctorRepo.GetAll();

            return View(doctors);
        }

        [HttpPost]
        public IActionResult Doctors(int[] selectedDoctors)
        {
            int count = 0;

            var doctors = doctorRepo.GetAll();

            foreach (var id in selectedDoctors)
            {
                var doctor = doctors
                    .FirstOrDefault(d => d.Id == id);

                if (doctor != null)
                {
                    doctor.IsAcceptingPatients = false;

                    doctorRepo.Update(doctor);

                    count++;
                }
            }

            TempData["Message"] =
                $"{count} doctor(s) updated.";

            return RedirectToAction("Doctors");
        }

        [HttpPost]
        public IActionResult MakeAvailable(int id)
        {
            var doctor = doctorRepo.GetById(id);

            if (doctor == null)
            {
                return NotFound();
            }

            doctor.IsAcceptingPatients = true;

            doctorRepo.Update(doctor);

            TempData["Message"] =
                $"{doctor.Name} is now accepting patients.";

            return RedirectToAction("Doctors");
        }
    }
}