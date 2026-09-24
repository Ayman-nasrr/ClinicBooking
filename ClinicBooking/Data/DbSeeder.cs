using ClinicBooking.Models;

namespace ClinicBooking.Data
{
    public static class DbSeeder
    {
        public static void Seed(ClinicContext db)
        {
            // =========================================
            // SEED DOCTORS
            // =========================================

            if (!db.Doctors.Any())
            {
                var doctors = new List<Doctor>
                {
                    new Doctor
                    {
                        Name = "Dr. Omar El-Sayed",
                        Phone = "01001112001",
                        Specialty = "Cardiology",
                        Bio = "Dr. Omar El-Sayed is a specialist in cardiology with a focus on preventive cardiac care and general cardiovascular health.",
                        YearsOfExperience = 14,
                        LicenceNumber = "CARD-1001",
                        IsAcceptingPatients = true
                    },

                    new Doctor
                    {
                        Name = "Dr. Nour Hassan",
                        Phone = "01001112002",
                        Specialty = "Cardiology",
                        Bio = "Dr. Nour Hassan focuses on cardiovascular diagnosis, follow-up care, and long-term heart health management.",
                        YearsOfExperience = 9,
                        LicenceNumber = "CARD-1002",
                        IsAcceptingPatients = true
                    },

                    new Doctor
                    {
                        Name = "Dr. Youssef Adel",
                        Phone = "01001112003",
                        Specialty = "Dentistry",
                        Bio = "Dr. Youssef Adel provides general dental care with a focus on preventive dentistry and restorative treatments.",
                        YearsOfExperience = 11,
                        LicenceNumber = "DENT-1001",
                        IsAcceptingPatients = true
                    },

                    new Doctor
                    {
                        Name = "Dr. Salma Nabil",
                        Phone = "01001112004",
                        Specialty = "Dentistry",
                        Bio = "Dr. Salma Nabil specializes in family dental care, oral health prevention, and routine restorative procedures.",
                        YearsOfExperience = 7,
                        LicenceNumber = "DENT-1002",
                        IsAcceptingPatients = false
                    },

                    new Doctor
                    {
                        Name = "Dr. Karim Tarek",
                        Phone = "01001112005",
                        Specialty = "Ophthalmology",
                        Bio = "Dr. Karim Tarek focuses on comprehensive eye examinations, vision care, and common ophthalmic conditions.",
                        YearsOfExperience = 13,
                        LicenceNumber = "OPHT-1001",
                        IsAcceptingPatients = true
                    },

                    new Doctor
                    {
                        Name = "Dr. Menna Ali",
                        Phone = "01001112006",
                        Specialty = "Ophthalmology",
                        Bio = "Dr. Menna Ali provides eye care services with a focus on diagnosis, follow-up, and preventive vision care.",
                        YearsOfExperience = 8,
                        LicenceNumber = "OPHT-1002",
                        IsAcceptingPatients = true
                    },

                    new Doctor
                    {
                        Name = "Dr. Adam Fathy",
                        Phone = "01001112007",
                        Specialty = "Pediatrics",
                        Bio = "Dr. Adam Fathy provides general pediatric care including routine checkups, growth monitoring, and common childhood conditions.",
                        YearsOfExperience = 10,
                        LicenceNumber = "PED-1001",
                        IsAcceptingPatients = true
                    },

                    new Doctor
                    {
                        Name = "Dr. Farah Samir",
                        Phone = "01001112008",
                        Specialty = "Dermatology",
                        Bio = "Dr. Farah Samir focuses on general dermatology, skin health, and the diagnosis and management of common skin conditions.",
                        YearsOfExperience = 12,
                        LicenceNumber = "DERM-1001",
                        IsAcceptingPatients = true
                    }
                };

                db.Doctors.AddRange(doctors);
            }


            // =========================================
            // SEED ADMIN USER
            // =========================================

            const string adminEmail =
                "admin@clinicbook.com";

            const string adminPasswordHash =
                "PBKDF2-SHA256$100000$veJFOKM3QT1gVGyNtTALMw==$EjY2LYzj4ocmBCf+GTm7yMKhiAq9deH+QnwPhodIZFE=";

            bool adminExists =
                db.Users.Any(u =>
                    u.Email == adminEmail);

            if (!adminExists)
            {
                var adminUser =
                    new User
                    {
                        Email = adminEmail,
                        PasswordHash = adminPasswordHash,
                        Role = "Admin"
                    };

                db.Users.Add(adminUser);
            }


            // =========================================
            // SAVE CHANGES
            // =========================================

            db.SaveChanges();
        }
    }
}