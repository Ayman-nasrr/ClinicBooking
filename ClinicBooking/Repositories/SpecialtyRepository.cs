using ClinicBooking.Data;

namespace ClinicBooking.Repositories
{
    public class SpecialtyRepository : ISpecialtyRepository
    {
        private readonly ClinicContext db;

        public SpecialtyRepository(ClinicContext db)
        {
            this.db = db;
        }

        public List<string> GetAll()
        {
            return db.Doctors
                .Select(d => d.Specialty)
                .Distinct()
                .ToList();
        }
    }
}