# Plan de Implementación: S2-HU-01 (Correcciones) y S2-HU-02 (Login y Sesiones)

Este plan aborda la estructuración final de la **Clean Architecture** para el Servicio 2 (Usuarios) y la integración completa del sistema de **Login con JWT y roles diferenciados** entre la Web API y el Frontend.

> [!IMPORTANT]
> Estamos trabajando en un entorno Orientado a Servicios (SOA). El backend (`Servicio_Clientes`) solo devolverá JSON (JWT) y el frontend (`FrontendLibreria`) consumirá esa API usando `HttpClient` y almacenará el token en una cookie.

## ⚠️ User Review Required
Se requiere tu aprobación para este plan de implementación que afecta a dos sub-proyectos distintos (`Servicio_Clientes` y `FrontendLibreria`) dentro de la solución `LibreriaJoelitoV2`.

## Proposed Changes

---

### Fase 1: Backend - Correcciones Clean Architecture (S2-HU-01)
*Directorio: `LibreriaJoelitoV2/Servicio_Clientes`*

#### [MODIFY] `Properties/launchSettings.json`
- Cambiar los puertos a `7002` (HTTPS) para estandarizar la API según los requerimientos.

#### [NEW] `Dominio/Models/Rol.cs`
- Crear entidad con propiedades: `Id`, `Nombre`, `Descripcion`.

#### [NEW] `Dominio/Models/Bitacora.cs`
- Crear entidad con propiedades de auditoría: `Id`, `IdUsuario`, `Accion`, `Tabla`, `Fecha`, `Descripcion`.

#### [MODIFY] `Dominio/Models/Usuario.cs`
- Agregar campos requeridos por HU-03/04: `bool MustChangePassword`, `bool Estado`.
- Agregar campos de auditoría exigidos por HU-05: `int IdEmpleadoCambio`, `DateTime FechaUltimaActualizacion`.

#### [MODIFY] `Infraestructura/Encryptacion/SimpleHasher.cs`
- Instalar el paquete NuGet `BCrypt.Net-Next` y modificar los métodos `Hash` y `Verify` para que usen BCrypt en lugar de SHA-256 (Requisito de la rúbrica).

#### [MODIFY] Movimiento de `TokenService.cs`
- **Mover** el archivo de `Aplicacion/Servicios` a `Infraestructura/ServiciosExternos` (el JWT es un detalle de infraestructura, la capa de Aplicación solo debe conocer su interfaz `ITokenService`).

#### [DELETE] `WeatherForecast.cs` y `Controllers/WeatherForecastController.cs`
- Limpieza de los archivos por defecto de la plantilla de .NET.

---

### Fase 2: Backend - Login API (S2-HU-02)
*Directorio: `LibreriaJoelitoV2/Servicio_Clientes`*

#### [MODIFY] `Infraestructura/Persistencia/FactoryProducts/UsuarioRepository.cs`
- Agregar el método `GetDatosLogin(string username)` para buscar el usuario por credenciales. (Asegurar que retorne el campo `MustChangePassword` y `Rol`).

#### [NEW] `Controllers/AuthController.cs`
- Crear el endpoint `POST /auth/login`.
- Inyectar `UsuarioServicio`.
- Validar credenciales y devolver el JWT si es exitoso, junto con el flag `MustChangePassword`.

---

### Fase 3: Frontend - Autenticación y Consumo API (S2-HU-02)
*Directorio: `LibreriaJoelitoV2/FrontendLibreria`*

#### [NEW] `Adapters/IUsuarioServiceAdapter.cs` y `Adapters/UsuarioServiceAdapter.cs`
- Implementar la clase que usará `HttpClient` para hacer una petición `POST` a `https://localhost:7002/auth/login`.

#### [MODIFY] `Program.cs`
- Configurar `HttpClient` apuntando a `https://localhost:7002`.
- Configurar autenticación basada en Cookies (`AddCookie`), configurando el sistema para extraer los claims del JWT guardado en la cookie y redirigir a `/Auth/Login` cuando falle el acceso.

#### [NEW] `Pages/Auth/Login.cshtml` y `.cs`
- UI corporativa y bonita para el inicio de sesión.
- Lógica en el `OnPost` para llamar al `UsuarioServiceAdapter.Login()`.
- Si es exitoso, guardar el JWT en una cookie HttpOnly (`Response.Cookies.Append(...)`) y firmar la identidad (`SignInAsync`).

#### [MODIFY] `Pages/Shared/_Layout.cshtml`
- Modificar el menú de navegación utilizando `@if (User.IsInRole("Admin"))`.
- Administrador ve todo; Empleado solo ve Productos/Clientes/Ventas.

## Verification Plan

### Automated Tests
- Ejecutar la API en el puerto 7002 y probar el endpoint `/auth/login` con Postman usando credenciales inválidas y válidas.
- Validar que el JWT devuelto contiene los claims correctos (`Id`, `NombreUsuario`, `Rol`).

### Manual Verification
1. **Frontend**: Ingresar a `/Auth/Login` y enviar credenciales.
2. Verificar en el navegador (DevTools -> Application) que la cookie HttpOnly con el JWT se ha guardado correctamente.
3. Verificar que el Layout cambie según el rol del usuario (Admin vs Empleado).
4. Intentar ingresar a una ruta protegida sin sesión y verificar la redirección automática a la pantalla de Login.
