# 🏨 Sistema de Gestión Hotelera - Hotel Premium

Sistema web completo desarrollado en ASP.NET Core MVC para la gestión integral de un hotel, incluyendo reservas, clientes, huéspedes, habitaciones y facturación.

## 📋 Tabla de Contenidos

- [Características](#-características)
- [Tecnologías Utilizadas](#-tecnologías-utilizadas)
- [Requisitos Previos](#-requisitos-previos)
- [Instalación](#-instalación)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Funcionalidades](#-funcionalidades)
- [Configuración](#-configuración)
- [Uso](#-uso)
- [Validaciones](#-validaciones)
- [Capturas de Pantalla](#-capturas-de-pantalla)
- [Contribución](#-contribución)
- [Licencia](#-licencia)

## ✨ Características

### Gestión de Clientes
- Registro y actualización de clientes
- Validación de datos personales
- Tipos de cliente: VIP, Frecuente, Corporativo, Regular, Premium
- Carga masiva desde archivos planos

### Gestión de Huéspedes
- Registro de huéspedes nacionales e internacionales
- Validación de nacionalidad para cálculo de IVA
- Carga masiva desde archivos planos

### Gestión de Habitaciones
- Tres tipos de habitaciones:
  - **Sencilla**: Pisos 2-4, $200.000/noche
  - **Ejecutiva**: Piso 5, $350.000/noche, con minibar
  - **Suite**: Piso 6, $500.000/noche, con minibar completo
- Visualización de disponibilidad en tiempo real
- Filtrado por tipo y estado

### Sistema de Reservas
- Creación de reservas con múltiples habitaciones
- Asignación de huéspedes a reservas
- Estados: Pendiente, Confirmada, Cancelada
- Cálculo automático de precios

### Recepción y Facturación
- Check-in y Check-out de huéspedes
- Generación automática de facturas
- Cálculo de:
  - Precio por noche
  - Seguro hotelero (2.5%)
  - IVA (19% solo para colombianos)
- Múltiples métodos de pago

### Validaciones Implementadas
- Nombres y apellidos: Solo letras, sin números
- Documentos: Máximo 10 dígitos, solo números
- Teléfonos: Exactamente 10 dígitos
- Nacionalidad: Solo letras, sin números
- Fechas: No permite fechas pasadas
- Emails: Validación de formato

## 🛠️ Tecnologías Utilizadas

- **.NET 9.0** - Framework principal
- **ASP.NET Core MVC** - Arquitectura web
- **Bootstrap 5** - Framework CSS responsivo
- **jQuery** - Manipulación del DOM
- **C#** - Lenguaje de programación
- **LINQ** - Consultas a colecciones
- **Programación Funcional** - Patrones modernos

## 📦 Requisitos Previos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Visual Studio 2022 o Visual Studio Code
- Git
- Navegador web moderno (Chrome, Firefox, Edge, Safari)

## 🚀 Instalación

### 1. Clonar el Repositorio

```bash
git clone https://github.com/SanTacrZ/hotel_MVC_ASP.NET.git
cd hotel_MVC_ASP.NET
```

### 2. Restaurar Dependencias

```bash
dotnet restore
```

### 3. Compilar el Proyecto

```bash
dotnet build
```

### 4. Ejecutar la Aplicación

```bash
dotnet run
```

La aplicación estará disponible en:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

## 📁 Estructura del Proyecto

```
hotel_web_final/
├── Controllers/          # Controladores MVC
│   ├── ClienteController.cs
│   ├── HuespedController.cs
│   ├── HabitacionController.cs
│   ├── ReservaController.cs
│   ├── RecepcionController.cs
│   └── HomeController.cs
├── Views/                # Vistas Razor
│   ├── Cliente/
│   ├── Huesped/
│   ├── Habitacion/
│   ├── Reserva/
│   ├── Recepcion/
│   ├── Home/
│   └── Shared/
├── Servicios/            # Lógica de negocio
│   ├── ClienteService.cs
│   ├── HuespedService.cs
│   ├── HabitacionService.cs
│   ├── ReservaService.cs
│   ├── HotelService.cs
│   ├── RecepcionService.cs
│   └── ValidacionService.cs
├── Models/               # Modelos de datos
├── wwwroot/              # Archivos estáticos
│   ├── css/
│   ├── js/
│   └── lib/
├── Arhivos/              # Archivos de datos (Clientes.txt, Huespedes.txt)
├── Program.cs            # Configuración de la aplicación
└── hotel_web_final.csproj
```

## 🎯 Funcionalidades

### Gestión de Clientes
- ✅ CRUD completo de clientes
- ✅ Validación de datos
- ✅ Carga desde archivos planos
- ✅ Búsqueda por documento

### Gestión de Huéspedes
- ✅ CRUD completo de huéspedes
- ✅ Validación de nacionalidad
- ✅ Carga desde archivos planos
- ✅ Gestión de descuentos semanales

### Gestión de Habitaciones
- ✅ Visualización de todas las habitaciones
- ✅ Filtrado por tipo y disponibilidad
- ✅ Detalles de cada habitación
- ✅ Inicialización automática (30 sencillas, 10 ejecutivas, 5 suites)

### Sistema de Reservas
- ✅ Creación de reservas
- ✅ Asignación de múltiples habitaciones
- ✅ Asignación de huéspedes
- ✅ Confirmación y cancelación
- ✅ Cálculo automático de precios

### Recepción
- ✅ Check-in de huéspedes
- ✅ Check-out con facturación
- ✅ Generación de facturas
- ✅ Cálculo de seguro e IVA
- ✅ Historial de facturas

## ⚙️ Configuración

### Archivos de Datos

Coloque los archivos de datos en la carpeta `Arhivos/`:

**Clientes.txt** (formato: separado por `|`)
```
CC|1234567890|Juan|Pérez|3001234567|juan@email.com|1234567890123456|VIP|Preferencias del cliente
```

**Huespedes.txt** (formato: separado por `|`)
```
CC|9876543210|María|González|3009876543|Colombia|maria@email.com
```

### Configuración de la Biblioteca

Asegúrese de que la DLL `biblioteca_hotel.dll` esté en la ruta correcta:
```
..\biblioteca_hotel\bin\Debug\biblioteca_hotel.dll
```

## 📝 Uso

### Inicialización

Al iniciar la aplicación, se inicializan automáticamente:
- 30 habitaciones sencillas (pisos 2-4)
- 10 habitaciones ejecutivas (piso 5)
- 5 suites (piso 6)

### Flujo de Trabajo

1. **Registrar Clientes/Huéspedes**: Use los formularios de registro
2. **Crear Reserva**: Seleccione cliente, fechas y habitaciones
3. **Check-in**: Al llegar el huésped, realice el check-in
4. **Check-out**: Al salir, realice el check-out y se generará la factura

## ✅ Validaciones

### Validaciones del Lado del Cliente (JavaScript)
- Nombres sin números
- Documentos con máximo 10 dígitos
- Teléfonos con exactamente 10 dígitos
- Nacionalidad sin números
- Fechas válidas

### Validaciones del Servidor (C#)
- Validación de duplicados
- Validación de fechas
- Validación de disponibilidad
- Validación de formato de datos

## 🎨 Diseño

- **Paleta de Colores Profesional**: Azul (#3498db), Gris oscuro (#2c3e50)
- **Diseño Responsivo**: Adaptado para móviles, tablets y desktop
- **Interfaz Moderna**: Cards, gradientes y animaciones suaves
- **UX Optimizada**: Validaciones en tiempo real y mensajes claros

## 📸 Capturas de Pantalla

> _Las capturas de pantalla se pueden agregar aquí_

## 🤝 Contribución

Las contribuciones son bienvenidas. Por favor:

1. Fork el proyecto
2. Cree una rama para su feature (`git checkout -b feature/AmazingFeature`)
3. Commit sus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abra un Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para más detalles.

## 👥 Autores

- **SanTacrZ** - *Desarrollo inicial* - [GitHub](https://github.com/SanTacrZ)

## 🙏 Agradecimientos

- Equipo de desarrollo
- Comunidad de ASP.NET Core
- Contribuidores de código abierto

---

⭐ Si este proyecto te fue útil, considera darle una estrella en GitHub!

