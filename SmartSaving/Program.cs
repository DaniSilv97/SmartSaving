using SmartSaving.Models;

namespace SmartSaving
{
    public class Program {
        public static string Conector = "";
        public static string SmtpIp = "";
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSession(s => s.IdleTimeout = TimeSpan.FromMinutes(20));

            builder.Services.AddMvc();
            var config = builder.Configuration.GetSection("Configuration").Get<Configuration>();

            Conector = config.ConnectionString;
            SmtpIp = config.SmtpIp;

            var app = builder.Build();

            app.UseSession();

            app.UseStaticFiles();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            app.Run();
        }
    }
}
