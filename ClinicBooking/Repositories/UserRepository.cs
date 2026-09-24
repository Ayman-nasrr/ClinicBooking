using ClinicBooking.Data;
using ClinicBooking.Models;

namespace ClinicBooking.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ClinicContext db;

        public UserRepository(ClinicContext db)
        {
            this.db = db;
        }

        public User? GetByEmail(string email)
        {
            return db.Users
                .FirstOrDefault(u => u.Email == email);
        }

        public bool EmailExists(string email)
        {
            return db.Users
                .Any(u => u.Email == email);
        }

        public void Add(User user)
        {
            db.Users.Add(user);
            db.SaveChanges();
        }
    }
}