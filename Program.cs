using hotel_web_final.Servicios;
using biblioteca_hotel.Clases;

namespace hotel_web_final
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSingleton<ClienteService>();
            builder.Services.AddSingleton<HuespedService>();
            builder.Services.AddSingleton<HabitacionService>();
            builder.Services.AddSingleton<HotelService>();
            builder.Services.AddScoped<ReservaService>();
            builder.Services.AddScoped<RecepcionService>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            var habitacionService = app.Services.GetRequiredService<HabitacionService>();
            habitacionService.InicializarHabitaciones();

            var hotelService = app.Services.GetRequiredService<HotelService>();
            hotelService.InicializarHotel();

            var clienteService = app.Services.GetRequiredService<ClienteService>();
            var huespedService = app.Services.GetRequiredService<HuespedService>();

            try
            {
                var rutaClientes = Path.Combine(Directory.GetCurrentDirectory(), "Arhivos", "Clientes.txt");
                if (File.Exists(rutaClientes))
                {
                    clienteService.Cargar(rutaClientes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar clientes: {ex.Message}");
            }

            try
            {
                var rutaHuespedes = Path.Combine(Directory.GetCurrentDirectory(), "Arhivos", "Huespedes.txt");
                if (File.Exists(rutaHuespedes))
                {
                    huespedService.Cargar(rutaHuespedes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar huéspedes: {ex.Message}");
            }

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
