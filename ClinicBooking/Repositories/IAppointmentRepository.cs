using ClinicBooking.Models;

namespace ClinicBooking.Repositories
{
    public interface IAppointmentRepository
    {
        List<Appointment> GetAll();

        Appointment? GetById(int id);

        void Add(Appointment appointment);

        void Update(Appointment appointment);

        void Delete(int id);
    }
}