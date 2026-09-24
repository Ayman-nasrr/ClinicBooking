using Microsoft.AspNetCore.Mvc;
using ClinicBooking.Models;
using ClinicBooking.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace ClinicBooking.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly IDoctorRepository doctorRepo;
        private readonly ISpecialtyRepository specialtyRepo;
        private readonly IAppointmentRepository appointmentRepo;

        public DoctorsController(
            IDoctorRepository doctorRepo,
            ISpecialtyRepository specialtyRepo,
            IAppointmentRepository appointmentRepo)
        {
            this.doctorRepo = doctorRepo;
            this.specialtyRepo = specialtyRepo;
            this.appointmentRepo = appointmentRepo;
        }


        // =========================================
        // DOCTORS INDEX
        // =========================================

        public IActionResult Index()
        {
            return Search(
                specialty: null,
                query: null,
                page: 1,
                sortBy: "name");
        }


        // =========================================
        // DOCTOR DETAILS
        // =========================================

        public IActionResult Details(int id)
        {
            var doctor = doctorRepo.GetById(id);

            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }


        // =========================================
        // SEARCH + FILTER + SORT + PAGINATION
        // =========================================

        public IActionResult Search(
            string? specialty,
            string? query,
            int page = 1,
            string sortBy = "name")
        {
            const int pageSize = 3;

            if (page < 1)
            {
                page = 1;
            }

            if (!sortBy.Equals(
                "experience",
                StringComparison.OrdinalIgnoreCase))
            {
                sortBy = "name";
            }

            int totalDoctors;

            var pagedResult =
                doctorRepo.Search(
                    specialty,
                    query,
                    page,
                    pageSize,
                    sortBy,
                    out totalDoctors);

            int totalPages =
                totalDoctors == 0
                    ? 1
                    : (int)Math.Ceiling(
                        totalDoctors / (double)pageSize);

            if (page > totalPages)
            {
                page = totalPages;

                pagedResult =
                    doctorRepo.Search(
                        specialty,
                        query,
                        page,
                        pageSize,
                        sortBy,
                        out totalDoctors);
            }

            ViewBag.TotalDoctors =
                totalDoctors;

            ViewBag.CurrentPage =
                page;

            ViewBag.TotalPages =
                totalPages;

            ViewBag.SearchQuery =
                query ?? string.Empty;

            ViewBag.SelectedSpecialty =
                specialty ?? string.Empty;

            ViewBag.SortBy =
                sortBy;

            ViewBag.Specialties =
                specialtyRepo.GetAll();

            ViewBag.IsFiltered =
                !string.IsNullOrWhiteSpace(query)
                ||
                !string.IsNullOrWhiteSpace(specialty);


            return View(
                "Index",
                pagedResult);
        }


        // =========================================
        // BOOK APPOINTMENT - GET
        // =========================================

        [Authorize]
        public IActionResult Book(int id)
        {
            var doctor =
                doctorRepo.GetById(id);

            if (doctor == null)
            {
                return NotFound();
            }


            // Do not allow booking with unavailable doctor
            if (!doctor.IsAcceptingPatients)
            {
                TempData["Message"] =
                    "This doctor is currently unavailable for booking.";

                return RedirectToAction("Index");
            }


            BookingViewModel vm =
                new BookingViewModel();

            vm.DoctorId =
                doctor.Id;

            vm.DoctorName =
                doctor.Name;

            vm.PhotoName =
                doctor.PhotoName;

            return View(vm);
        }


        // =========================================
        // BOOK APPOINTMENT - POST
        // =========================================

        [Authorize]
        [HttpPost]
        public IActionResult Book(
            BookingViewModel vm)
        {
            // Get the real doctor from database
            // instead of trusting submitted doctor data.
            var doctor =
                doctorRepo.GetById(vm.DoctorId);

            if (doctor == null)
            {
                return NotFound();
            }


            // Re-check doctor availability
            if (!doctor.IsAcceptingPatients)
            {
                TempData["Message"] =
                    "This doctor is currently unavailable for booking.";

                return RedirectToAction("Index");
            }


            // Validation
            if (!ModelState.IsValid)
            {
                vm.DoctorName =
                    doctor.Name;

                vm.PhotoName =
                    doctor.PhotoName;

                return View(vm);
            }


            // Date/time must be in the future
            if (vm.SlotDateTime <= DateTime.Now)
            {
                ModelState.AddModelError(
                    nameof(vm.SlotDateTime),
                    "Please choose a future date and time.");

                vm.DoctorName =
                    doctor.Name;

                vm.PhotoName =
                    doctor.PhotoName;

                return View(vm);
            }


            // Prevent duplicate appointment slot
            var existingAppointment =
                appointmentRepo
                    .GetAll()
                    .FirstOrDefault(a =>
                        a.DoctorId ==
                            vm.DoctorId
                        &&
                        a.SlotDateTime ==
                            vm.SlotDateTime);

            if (existingAppointment != null)
            {
                ModelState.AddModelError(
                    nameof(vm.SlotDateTime),
                    "This appointment slot is already booked.");

                vm.DoctorName =
                    doctor.Name;

                vm.PhotoName =
                    doctor.PhotoName;

                return View(vm);
            }


            // Create appointment
            Appointment appointment =
                new Appointment
                {
                    DoctorId =
                        doctor.Id,

                    PatientName =
                        vm.PatientName,

                    PatientPhone =
                        vm.PatientPhone,

                    SlotDateTime =
                        vm.SlotDateTime,

                    Notes =
                        vm.Notes
                };


            // Save appointment to database
            appointmentRepo.Add(
                appointment);


            TempData["Message"] =
                $"Booking confirmed for {doctor.Name} on {vm.SlotDateTime}";


            return RedirectToAction(
                "Index");
        }


        // =========================================
        // CREATE DOCTOR
        // =========================================

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Specialties =
                specialtyRepo.GetAll();

            return View();
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create(
            DoctorFormViewModel vm,
            IFormFile? photoFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Specialties =
                    specialtyRepo.GetAll();

                return View(vm);
            }


            // =====================================
            // PHOTO VALIDATION
            // =====================================

            string? photoName = null;

            if (photoFile != null &&
                photoFile.Length > 0)
            {
                string[] allowedExtensions =
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".webp"
                };


                string extension =
                    Path.GetExtension(
                        photoFile.FileName)
                        .ToLowerInvariant();


                // Validate extension
                if (!allowedExtensions.Contains(
                    extension))
                {
                    ModelState.AddModelError(
                        "photoFile",
                        "Only JPG, JPEG, PNG, and WEBP images are allowed.");

                    ViewBag.Specialties =
                        specialtyRepo.GetAll();

                    return View(vm);
                }


                // Validate file size
                const long maxFileSize =
                    5 * 1024 * 1024;


                if (photoFile.Length >
                    maxFileSize)
                {
                    ModelState.AddModelError(
                        "photoFile",
                        "Image size must not exceed 5 MB.");

                    ViewBag.Specialties =
                        specialtyRepo.GetAll();

                    return View(vm);
                }


                // Generate unique file name
                photoName =
                    Guid.NewGuid().ToString()
                    + extension;


                string doctorsDirectory =
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/images/doctors");

                Directory.CreateDirectory(doctorsDirectory);

                string path =
                    Path.Combine(
                        doctorsDirectory,
                        photoName);

                // Save image
                using (var stream =
                    new FileStream(
                        path,
                        FileMode.Create))
                {
                    photoFile.CopyTo(
                        stream);
                }
            }


            // =====================================
            // MAP VIEWMODEL TO DOCTOR
            // =====================================

            Doctor doctor =
                new Doctor
                {
                    Name =
                        vm.Name,

                    Phone =
                        vm.Phone,

                    LicenceNumber =
                        vm.LicenceNumber,

                    Specialty =
                        vm.Specialty,

                    Bio =
                        vm.Bio,

                    YearsOfExperience =
                        vm.YearsOfExperience,

                    IsAcceptingPatients =
                        vm.IsAcceptingPatients,

                    PhotoName =
                        photoName
                };


            doctorRepo.Add(
                doctor);

            return RedirectToAction(
                "Index");
        }


        // =========================================
        // EDIT DOCTOR
        // =========================================

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(
            int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }


            var doctor =
                doctorRepo.GetById(
                    id.Value);

            if (doctor == null)
            {
                return NotFound();
            }


            ViewBag.Specialties =
                specialtyRepo.GetAll();


            ViewBag.CurrentPhotoName =
                doctor.PhotoName;


            DoctorFormViewModel vm =
                new DoctorFormViewModel
                {
                    Id =
                        doctor.Id,

                    Name =
                        doctor.Name,

                    Phone =
                        doctor.Phone,

                    LicenceNumber =
                        doctor.LicenceNumber,

                    Specialty =
                        doctor.Specialty,

                    Bio =
                        doctor.Bio,

                    YearsOfExperience =
                        doctor.YearsOfExperience,

                    IsAcceptingPatients =
                        doctor.IsAcceptingPatients
                };


            return View(vm);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Edit(
            DoctorFormViewModel vm,
            IFormFile? photoFile)
        {
            // Get the real doctor from the database.
            var existingDoctor =
                doctorRepo.GetById(
                    vm.Id);

            if (existingDoctor == null)
            {
                return NotFound();
            }


            if (!ModelState.IsValid)
            {
                ViewBag.Specialties =
                    specialtyRepo.GetAll();

                ViewBag.CurrentPhotoName =
                    existingDoctor.PhotoName;

                return View(vm);
            }


            // Get the existing photo name
            // from the database only.
            string oldPhotoName =
                existingDoctor.PhotoName ??
                string.Empty;


            // =====================================
            // PHOTO VALIDATION + REPLACEMENT
            // =====================================

            if (photoFile != null &&
                photoFile.Length > 0)
            {
                string[] allowedExtensions =
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".webp"
                };


                string extension =
                    Path.GetExtension(
                        photoFile.FileName)
                        .ToLowerInvariant();


                // Validate extension
                if (!allowedExtensions.Contains(
                    extension))
                {
                    ModelState.AddModelError(
                        "photoFile",
                        "Only JPG, JPEG, PNG, and WEBP images are allowed.");

                    ViewBag.Specialties =
                        specialtyRepo.GetAll();

                    ViewBag.CurrentPhotoName =
                        existingDoctor.PhotoName;

                    return View(vm);
                }


                // Validate file size
                const long maxFileSize =
                    5 * 1024 * 1024;


                if (photoFile.Length >
                    maxFileSize)
                {
                    ModelState.AddModelError(
                        "photoFile",
                        "Image size must not exceed 5 MB.");

                    ViewBag.Specialties =
                        specialtyRepo.GetAll();

                    ViewBag.CurrentPhotoName =
                        existingDoctor.PhotoName;

                    return View(vm);
                }


                // Generate new unique file name
                string fileName =
                    Guid.NewGuid().ToString()
                    + extension;


                string doctorsDirectory =
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/images/doctors");

                Directory.CreateDirectory(doctorsDirectory);

                string path =
                    Path.Combine(
                        doctorsDirectory,
                        fileName);


                // Save new photo first
                using (var stream =
                    new FileStream(
                        path,
                        FileMode.Create))
                {
                    photoFile.CopyTo(
                        stream);
                }


                // Store the new photo name
                existingDoctor.PhotoName =
                    fileName;


                // Delete old photo using
                // the trusted database value.
                if (!string.IsNullOrEmpty(
                    oldPhotoName))
                {
                    string oldPhotoPath =
                        Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot/images/doctors",
                            oldPhotoName);


                    if (System.IO.File.Exists(
                        oldPhotoPath))
                    {
                        System.IO.File.Delete(
                            oldPhotoPath);
                    }
                }
            }


            // =====================================
            // MAP VIEWMODEL TO EXISTING DOCTOR
            // =====================================

            existingDoctor.Name =
                vm.Name;

            existingDoctor.Phone =
                vm.Phone;

            existingDoctor.LicenceNumber =
                vm.LicenceNumber;

            existingDoctor.Specialty =
                vm.Specialty;

            existingDoctor.Bio =
                vm.Bio;

            existingDoctor.YearsOfExperience =
                vm.YearsOfExperience;

            existingDoctor.IsAcceptingPatients =
                vm.IsAcceptingPatients;


            doctorRepo.Update(
                existingDoctor);

            return RedirectToAction(
                "Index");
        }


        // =========================================
        // DELETE DOCTOR
        // =========================================

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(
            int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }


            var doctor =
                doctorRepo.GetById(
                    id.Value);

            if (doctor == null)
            {
                return NotFound();
            }


            return View(doctor);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Delete(
            Doctor doctor)
        {
            var existingDoctor =
                doctorRepo.GetById(
                    doctor.Id);


            if (existingDoctor == null)
            {
                return NotFound();
            }


            string photoName =
                existingDoctor.PhotoName ??
                string.Empty;


            // Delete doctor from database
            doctorRepo.Delete(
                existingDoctor.Id);


            // Delete doctor's image
            if (!string.IsNullOrEmpty(
                photoName))
            {
                string photoPath =
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/images/doctors",
                        photoName);


                if (System.IO.File.Exists(
                    photoPath))
                {
                    System.IO.File.Delete(
                        photoPath);
                }
            }


            return RedirectToAction(
                "Index");
        }


        // =========================================
        // LICENCE VALIDATION
        // =========================================

        public IActionResult CheckLicenceNumber(
            string licenceNumber,
            int? id)
        {
            bool exists =
                doctorRepo.LicenceExists(
                    licenceNumber,
                    id);


            if (exists)
            {
                return Json(false);
            }


            return Json(true);
        }
    }
}