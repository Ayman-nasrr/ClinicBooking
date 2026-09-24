using ClinicBooking.Models;

namespace ClinicBooking.Repositories
{
    public interface IDoctorRepository
    {
        List<Doctor> GetAll();

        Doctor? GetById(int id);

        void Add(Doctor doctor);

        void Update(Doctor doctor);

        void Delete(int id);

        bool LicenceExists(string licenceNumber, int? doctorId = null);

        List<Doctor> Search(
            string? specialty,
            string? query,
            int page,
            int pageSize,
            string sortBy,
            out int totalDoctors);
    }
}