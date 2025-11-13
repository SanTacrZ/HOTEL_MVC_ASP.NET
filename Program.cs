using hotel_web_final.Servicios;
using biblioteca_hotel.Clases;

namespace hotel_web_final
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews(options =>
            {
                // Agregar filtro de autorización global
                options.Filters.Add<hotel_web_final.Filters.AuthorizationFilter>();
            });
            builder.Services.AddHttpContextAccessor();

            // Configurar sesiones para el sistema de autenticación
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // --- CORRECCIÓN 1: Todos los servicios deben ser Singleton ---
            // Así actúan como una base de datos en memoria.

            // Servicios base (sin dependencias)
            builder.Services.AddSingleton<AuditoriaService>();      // Sin dependencias
            builder.Services.AddSingleton<AuthService>();           // Depende de AuditoriaService
            builder.Services.AddSingleton<NotificacionService>();   // Depende de AuditoriaService
            builder.Services.AddSingleton<HabitacionService>();     // Sin dependencias
            builder.Services.AddSingleton<HotelService>();          // Sin dependencias

            // Servicios con dependencias de AuditoriaService
            builder.Services.AddSingleton<ClienteService>();
            builder.Services.AddSingleton<HuespedService>();
            builder.Services.AddSingleton<MinibarService>();

            // Servicios que dependen de otros servicios
            builder.Services.AddSingleton<ReservaService>();        // Depende de Habitacion, Cliente, Huesped, Auditoria
            builder.Services.AddSingleton<RecepcionService>();      // Depende de Reserva, Hotel, Auditoria
            builder.Services.AddSingleton<ServicioAdicionalService>(); // Depende de Reserva, Habitacion, Auditoria

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            // --- Carga de datos inicial al arrancar la app ---

            // 1. Inicializar Habitaciones y Hotel
            // (Usamos un scope para obtener los servicios recién creados)
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();

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
                        logger.LogInformation("Clientes cargados exitosamente desde {RutaArchivo}", rutaClientes);
                    }
                    else
                    {
                        logger.LogWarning("No se encontró el archivo de clientes en {RutaArchivo}", rutaClientes);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error al cargar clientes desde archivo");
                }

                // 3. Cargar Huéspedes desde archivo
                try
                {
                    var huespedService = services.GetRequiredService<HuespedService>();
                    var rutaHuespedes = Path.Combine(Directory.GetCurrentDirectory(), "Arhivos", "Huespedes.txt");
                    if (File.Exists(rutaHuespedes))
                    {
                        huespedService.Cargar(rutaHuespedes);
                        logger.LogInformation("Huéspedes cargados exitosamente desde {RutaArchivo}", rutaHuespedes);
                    }
                    else
                    {
                        logger.LogWarning("No se encontró el archivo de huéspedes en {RutaArchivo}", rutaHuespedes);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error al cargar huéspedes desde archivo");
                }
            }
            
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Login}/{id?}");

            app.Run();
        }
    }
}