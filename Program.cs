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

            // --- CORRECCIÓN 1: Todos los servicios deben ser Singleton ---
            // Así actúan como una base de datos en memoria.
            builder.Services.AddSingleton<ClienteService>();
            builder.Services.AddSingleton<HuespedService>();
            builder.Services.AddSingleton<HabitacionService>();
            builder.Services.AddSingleton<HotelService>();
            builder.Services.AddSingleton<ReservaService>();    // <-- CORREGIDO
            builder.Services.AddSingleton<RecepcionService>();  // <-- CORREGIDO

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

            // --- Carga de datos inicial al arrancar la app ---

            // 1. Inicializar Habitaciones y Hotel
            // (Usamos un scope para obtener los servicios recién creados)
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var habitacionService = services.GetRequiredService<HabitacionService>();
                habitacionService.InicializarHabitaciones();

                var hotelService = services.GetRequiredService<HotelService>();
                hotelService.InicializarHotel();

                // 2. Cargar Clientes desde archivo
                try
                {
                    var clienteService = services.GetRequiredService<ClienteService>();
                    var rutaClientes = Path.Combine(Directory.GetCurrentDirectory(), "Arhivos", "Clientes.txt");
                    if (File.Exists(rutaClientes))
                    {
                        clienteService.Cargar(rutaClientes);
                        Console.WriteLine($"Éxito: Clientes cargados desde {rutaClientes}");
                    }
                    else
                    {
                        Console.WriteLine($"ADVERTENCIA: No se encontró el archivo de clientes en {rutaClientes}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al cargar clientes: {ex.Message}");
                }

                // 3. Cargar Huéspedes desde archivo
                try
                {
                    var huespedService = services.GetRequiredService<HuespedService>();
                    var rutaHuespedes = Path.Combine(Directory.GetCurrentDirectory(), "Arhivos", "Huespedes.txt");
                    if (File.Exists(rutaHuespedes))
                    {
                        huespedService.Cargar(rutaHuespedes);
                        Console.WriteLine($"Éxito: Huéspedes cargados desde {rutaHuespedes}");
                    }
                    else
                    {
                        Console.WriteLine($"ADVERTENCIA: No se encontró el archivo de huéspedes en {rutaHuespedes}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al cargar huéspedes: {ex.Message}");
                }
            }
            
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}