using ClinicBooking.Models;

namespace ClinicBooking.Repositories
{
    public interface IUserRepository
    {
        User? GetByEmail(string email);

        bool EmailExists(string email);

        void Add(User user);
    }
}