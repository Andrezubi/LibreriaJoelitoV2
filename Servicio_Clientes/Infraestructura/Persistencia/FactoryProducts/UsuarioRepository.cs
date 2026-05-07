using Servicio_Clientes.Dominio.Interfaces;
using Servicio_Clientes.Dominio.Models;
using MySql.Data.MySqlClient;
using System.Data;
using Servicio_Clientes.Infraestructura.Persistencia.BD;

namespace Servicio_Clientes.Infraestructura.Persistencia.FactoryProducts
{
    public class UsuarioRepository : ConexionBD, IRepositorio<Usuario>
    {
        public int Insertar(Usuario t)
        {
            string query = @"INSERT INTO Usuario (
                    Nombre,
                    ApellidoPaterno,
                    ApellidoMaterno,
                    Ci,
                    Complemento,
                    FechaNacimiento,
                    Email,
                    DireccionDomicilio,
                    Telefono,
                    FechaIngreso,
                    Rol,
                    Username,
                    Password,
                    MustChangePassword,
                    IdUsuario,
                    Estado
                ) VALUES (
                    @nombre,
                    @apellidoPaterno,
                    @apellidoMaterno,
                    @ci,
                    @extensionCi,
                    @fechaNacimiento,
                    @email,
                    @direccionDomicilio,
                    @telefono,
                    @fechaIngreso,
                    @rol,
                    @username,
                    @password,
                    @mustChangePassword,
                    @idusuario,
                    1
                );";

            MySqlCommand command = new MySqlCommand(query);
            command.Parameters.AddWithValue("@nombre", t.Nombre);
            command.Parameters.AddWithValue("@apellidoPaterno", t.ApellidoPaterno);
            command.Parameters.AddWithValue("@apellidoMaterno", t.ApellidoMaterno);
            command.Parameters.AddWithValue("@ci", t.Ci);
            command.Parameters.AddWithValue("@extensionCi", t.Complemento ?? "");
            command.Parameters.AddWithValue("@email", t.Email);
            command.Parameters.AddWithValue("@direccionDomicilio", t.DireccionDomicilio);
            command.Parameters.AddWithValue("@telefono", t.Telefono);
            command.Parameters.AddWithValue("@fechaIngreso", t.FechaIngreso.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@fechaNacimiento", t.FechaNacimiento.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@rol", t.Rol);
            command.Parameters.AddWithValue("@username", t.NombreUsuario);
            command.Parameters.AddWithValue("@password", t.Contrasena);
            command.Parameters.AddWithValue("@mustChangePassword", t.DebeCambiarContrasena);
            command.Parameters.AddWithValue("@idusuario", t.IdUsuario);

            return ExecuteNonQuery(command);
        }

        public int Actualizar(Usuario t)
        {
            string query = @"UPDATE Usuario
                     SET Nombre = @nombre, 
                         ApellidoPaterno = @apellidoPaterno, 
                         ApellidoMaterno = @apellidoMaterno, 
                         CI = @ci, 
                         Complemento = @complemento,
                         Email = @email, 
                         DireccionDomicilio = @direccion,
                         Telefono = @telefono,
                         FechaNacimiento = @fechaNacimiento,
                         FechaIngreso = @fechaIngreso,
                         Rol = @rol,
                         FechaUltimaActualizacion = NOW() 
                     WHERE id = @id;";

            MySqlCommand command = new MySqlCommand(query);
            command.Parameters.AddWithValue("@nombre", t.Nombre);
            command.Parameters.AddWithValue("@apellidoPaterno", t.ApellidoPaterno);
            command.Parameters.AddWithValue("@apellidoMaterno", t.ApellidoMaterno);
            command.Parameters.AddWithValue("@ci", t.Ci);
            command.Parameters.AddWithValue("@complemento", t.Complemento ?? "");
            command.Parameters.AddWithValue("@email", t.Email);
            command.Parameters.AddWithValue("@direccion", t.DireccionDomicilio);
            command.Parameters.AddWithValue("@telefono", t.Telefono);
            command.Parameters.AddWithValue("@fechaNacimiento", t.FechaNacimiento.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@fechaIngreso", t.FechaIngreso.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@rol", t.Rol);
            command.Parameters.AddWithValue("@id", t.Id);

            return ExecuteNonQuery(command);
        }

        public int Eliminar(Usuario t)
        {
            string query = "UPDATE Usuario SET Estado = FALSE, FechaUltimaActualizacion = CURRENT_TIMESTAMP, IdUsuario = @idUsuario WHERE Id = @Id;";
            MySqlCommand command = new MySqlCommand(query);
            command.Parameters.AddWithValue("@id", t.Id);
            command.Parameters.AddWithValue("@idUsuario", t.IdUsuario);
            return ExecuteNonQuery(command);
        }

        public DataTable ObtenerTodo()
        {
            string query = @"SELECT Id, Nombre, ApellidoPaterno, ApellidoMaterno, Ci, Complemento, DATE_FORMAT(FechaNacimiento, '%Y-%m-%d') AS FechaNacimiento, Email, DireccionDomicilio, Rol, Telefono, DATE_FORMAT(FechaIngreso, '%Y-%m-%d') AS FechaIngreso, Username
                    FROM Usuario
                    WHERE Estado = 1
                    ORDER BY Nombre ASC;";
            MySqlCommand command = new MySqlCommand(query);
            return ExecuteReturningDataTable(command);
        }

        public DataRow? ObtenerPorId(int id)
        {
            string query = @"SELECT Id, Nombre, ApellidoPaterno, ApellidoMaterno, Ci, Complemento, DATE_FORMAT(FechaNacimiento, '%d/%m/%Y') AS FechaNacimiento, Email, DireccionDomicilio, Rol, Telefono, DATE_FORMAT(FechaIngreso, '%Y-%m-%d') AS FechaIngreso, Username
                    FROM Usuario
                    WHERE Id = @id AND Estado = 1;";
            MySqlCommand command = new MySqlCommand(query);
            command.Parameters.AddWithValue("@id", id);
            
            DataTable dt = ExecuteReturningDataTable(command);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0];
            }
            return null;
        }

        public bool ExisteDuplicado(Usuario empleado)
        {
            MySqlCommand cmd = new MySqlCommand(@"
                SELECT COUNT(*) FROM Usuario
                WHERE Ci          = @ci
                  AND Complemento = @complemento
                  AND Id         <> @id
                  AND Estado      = 1");

            cmd.Parameters.AddWithValue("@ci", empleado.Ci);
            cmd.Parameters.AddWithValue("@complemento", empleado.Complemento ?? string.Empty);
            cmd.Parameters.AddWithValue("@id", empleado.Id);

            return Convert.ToInt32(ExecuteScalar(cmd)) > 0;
        }

        public bool ExisteUsername(string nombreUsuario)
        {
            MySqlCommand cmd = new MySqlCommand(@"
                SELECT COUNT(*) FROM Usuario
                WHERE username = @username
                AND Estado = 1");
            cmd.Parameters.AddWithValue("@username", nombreUsuario);
            return Convert.ToInt32(ExecuteScalar(cmd)) > 0;
        }

        public string? ObtenerContrasenaPorNombreUsuario(string nombreUsuario)
        {
            MySqlCommand cmd = new MySqlCommand(@"
                SELECT Password 
                FROM Usuario
                WHERE Username = @username
                AND Estado = 1
                LIMIT 1");

            cmd.Parameters.AddWithValue("@username", nombreUsuario);

            using (var reader = ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    return reader["password"].ToString();
                }
            }

            return null;
        }

        public Usuario? ObtenerDatosLogin(string nombreUsuario)
        {
            string query = "SELECT Password, Rol, Id, MustChangePassword FROM Usuario WHERE Username = @NombreUsuario AND Estado = 1 LIMIT 1";
            MySqlCommand command = new MySqlCommand(query);
            command.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);

            using (var reader = ExecuteReader(command))
            {
                if (reader.Read())
                {
                    return new Usuario
                    {
                        NombreUsuario = nombreUsuario,
                        Contrasena = reader["Password"].ToString(),
                        Rol = reader["Rol"].ToString(),
                        Id = int.Parse(reader["Id"].ToString() ?? ""),
                        DebeCambiarContrasena = Convert.ToBoolean(reader["MustChangePassword"])
                    };
                }
            }
            return null;
        }
    }
}
