using ClinicBooking.Data;
using ClinicBooking.Models;

namespace ClinicBooking.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ClinicContext db;

        public AppointmentRepository(ClinicContext db)
        {
            this.db = db;
        }

        public List<Appointment> GetAll()
        {
            return db.Appointments.ToList();
        }

        public Appointment? GetById(int id)
        {
            return db.Appointments
                .FirstOrDefault(a => a.Id == id);
        }

        public void Add(Appointment appointment)
        {
            db.Appointments.Add(appointment);
            db.SaveChanges();
        }

        public void Update(Appointment appointment)
        {
            db.Appointments.Update(appointment);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            var appointment =
                db.Appointments
                    .FirstOrDefault(a => a.Id == id);

            if (appointment != null)
            {
                db.Appointments.Remove(appointment);
                db.SaveChanges();
            }
        }
    }
}