using ClinicBooking.Data;
using ClinicBooking.Models;

namespace ClinicBooking.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ClinicContext db;

        public DoctorRepository(ClinicContext db)
        {
            this.db = db;
        }

        public List<Doctor> GetAll()
        {
            return db.Doctors.ToList();
        }

        public Doctor? GetById(int id)
        {
            return db.Doctors
                .FirstOrDefault(d => d.Id == id);
        }

        public void Add(Doctor doctor)
        {
            db.Doctors.Add(doctor);
            db.SaveChanges();
        }

        public void Update(Doctor doctor)
        {
            db.Doctors.Update(doctor);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            var doctor =
                db.Doctors.FirstOrDefault(d => d.Id == id);

            if (doctor != null)
            {
                db.Doctors.Remove(doctor);
                db.SaveChanges();
            }
        }

        public bool LicenceExists(
            string licenceNumber,
            int? doctorId = null)
        {
            return db.Doctors
                .Any(d =>
                    d.LicenceNumber == licenceNumber &&
                    (!doctorId.HasValue ||
                     d.Id != doctorId.Value));
        }

        public List<Doctor> Search(
            string? specialty,
            string? query,
            int page,
            int pageSize,
            string sortBy,
            out int totalDoctors)
        {
            var result = db.Doctors.AsQueryable();

            // Search by doctor name or specialty
            if (!string.IsNullOrWhiteSpace(query))
            {
                result = result.Where(d =>
                    d.Name.Contains(query) ||
                    d.Specialty.Contains(query));
            }

            // Filter by specialty
            if (!string.IsNullOrWhiteSpace(specialty))
            {
                result = result.Where(d =>
                    d.Specialty == specialty);
            }

            // Total count before pagination
            totalDoctors = result.Count();

            // Sorting
            if (sortBy.Equals(
                "experience",
                StringComparison.OrdinalIgnoreCase))
            {
                result = result
                    .OrderByDescending(d => d.YearsOfExperience);
            }
            else
            {
                result = result
                    .OrderBy(d => d.Name);
            }

            // Pagination
            return result
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
    }
}