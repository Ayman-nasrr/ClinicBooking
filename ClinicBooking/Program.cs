using ClinicBooking.Data;
using ClinicBooking.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using ClinicBooking.Middleware;
using ClinicBooking.Services;

namespace ClinicBooking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // =========================================
            // DATABASE
            // =========================================

            builder.Services.AddDbContext<ClinicContext>(
                options =>
                {
                    options.UseSqlServer(
                        builder.Configuration.GetConnectionString(
                            "DefaultConnection"));
                });


            // =========================================
            // REPOSITORIES
            // =========================================

            builder.Services.AddScoped<
                IDoctorRepository,
                DoctorRepository>();

            builder.Services.AddScoped<
                ISpecialtyRepository,
                SpecialtyRepository>();

            builder.Services.AddScoped<
                IAppointmentRepository,
                AppointmentRepository>();

            builder.Services.AddScoped<
                IUserRepository,
                UserRepository>();

            builder.Services.AddScoped<PasswordService>();


            // =========================================
            // AUTHENTICATION
            // =========================================

            builder.Services
                .AddAuthentication(
                    CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath =
                        "/Account/Login";

                    options.AccessDeniedPath =
                        "/Account/AccessDenied";

                    options.ExpireTimeSpan =
                        TimeSpan.FromHours(8);

                    options.SlidingExpiration =
                        true;
                });


            // =========================================
            // SESSION
            // =========================================

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout =
                    TimeSpan.FromMinutes(30);

                options.Cookie.HttpOnly =
                    true;

                options.Cookie.IsEssential =
                    true;
            });


            // =========================================
            // MVC
            // =========================================

            builder.Services.AddControllersWithViews();


            // =========================================
            // BUILD APP
            // =========================================

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ClinicContext>();

                DbSeeder.Seed(db);
            }

            // =========================================
            // HTTP REQUEST PIPELINE
            // =========================================

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler(
                    "/Home/Error");

                app.UseHsts();
            }


            app.UseMiddleware<RequestLoggingMiddleware>();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();

            app.UseAuthorization();


            // =========================================
            // ROUTING
            // =========================================

            app.MapControllerRoute(
                name: "default",
                pattern:
                    "{controller=Home}/{action=Index}/{id:int:min(1)?}");


            app.Run();
        }
    }
}