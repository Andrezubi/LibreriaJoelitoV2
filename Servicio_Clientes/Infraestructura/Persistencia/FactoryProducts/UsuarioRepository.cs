using Servicio_Clientes.Aplicacion.Interfaces;
using Servicio_Clientes.Dominio.Models;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Servicio_Clientes.Infraestructura.Persistencia.FactoryProducts
{
    public class UsuarioRepository : RepositorioBD, IUsuarioRepository, IRepository<Usuario>
    {
        public int Insert(Usuario t)
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
                    IdUsuario
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
                    @idusuario
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
            command.Parameters.AddWithValue("@username", t.Username);
            command.Parameters.AddWithValue("@password", t.Password);
            command.Parameters.AddWithValue("@idusuario", t.IdUsuario);

            return ExecuteNonQuery(command);
        }
        public int Update(Usuario t)
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
            command.Parameters.AddWithValue("@id", t.Id);


            return ExecuteNonQuery(command);
        }
        public int Delete(Usuario t)
        {
            string query = "UPDATE Usuario SET Estado = FALSE,FechaUltimaActualizacion = CURRENT_TIMESTAMP, IdUsuario = @idUsuario WHERE Id = @Id;";
            MySqlCommand command = new MySqlCommand(query);
            command.Parameters.AddWithValue("@id", t.Id);
            command.Parameters.AddWithValue("@idUsuario", t.IdUsuario);
            return ExecuteNonQuery(command);

        }
        public List<T> GetAll()
        {
            string query = @"SELECT Id, Nombre, ApellidoPaterno, ApellidoMaterno, Ci,Complemento, DATE_FORMAT(FechaNacimiento, '%Y-%m-%d') AS FechaNacimiento,Email, DireccionDomicilio,Rol, Telefono, DATE_FORMAT(FechaIngreso, '%Y-%m-%d') AS FechaIngreso
                    FROM Usuario
                    WHERE estado = 1
                            ORDER BY 2;
                            ";
            MySqlCommand command = new MySqlCommand(query);
            return ExecuteList(command, reader => new Usuario
            {
                Id = reader.GetInt32("Id"),
                Nombre = reader.GetString("Nombre"),
                ApellidoPaterno = reader.GetString("ApellidoPaterno"),
                ApellidoMaterno = reader.IsDBNull(reader.GetOrdinal("ApellidoMaterno")) ? null : reader.GetString("ApellidoMaterno"),
                Ci = reader.GetString("Ci"),
                Complemento = reader.IsDBNull(reader.GetOrdinal("Complemento")) ? null : reader.GetString("Complemento"),
                FechaNacimiento = DateTime.Parse(reader["FechaNacimiento"].ToString()),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString("Email"),
                DireccionDomicilio = reader.IsDBNull(reader.GetOrdinal("DireccionDomicilio")) ? null : reader.GetString("DireccionDomicilio"),
                Rol = reader.GetString("Rol"),
                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString("Telefono"),
                FechaIngreso = DateTime.Parse(reader["FechaIngreso"].ToString())
            });
        }
        //public List<T> GetById(int id)
        //{
        //    return ExecuteList(new MySqlCommand("SELECT Id, Nombre, ApellidoPaterno, ApellidoMaterno, Ci,Complemento, DATE_FORMAT(FechaNacimiento, '%Y-%m-%d') AS FechaNacimiento,Email, DireccionDomicilio,Rol, Telefono, DATE_FORMAT(FechaIngreso, '%Y-%m-%d') AS FechaIngreso FROM Usuario WHERE Id = @id AND Estado = 1"), reader => new Usuario
        //    {
        //        Id = reader.GetInt32("Id"),
        //        Nombre = reader.GetString("Nombre"),
        //        ApellidoPaterno = reader.GetString("ApellidoPaterno"),
        //        ApellidoMaterno = reader.IsDBNull(reader.GetOrdinal("ApellidoMaterno")) ? null : reader.GetString("ApellidoMaterno"),
        //        Ci = reader.GetString("Ci"),
        //        Complemento = reader.IsDBNull(reader.GetOrdinal("Complemento")) ? null : reader.GetString("Complemento"),
        //        FechaNacimiento = DateTime.Parse(reader["FechaNacimiento"].ToString()),
        //        Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString("Email"),
        //        DireccionDomicilio = reader.IsDBNull(reader.GetOrdinal("DireccionDomicilio")) ? null : reader.GetString("DireccionDomicilio"),
        //        Rol = reader.GetString("Rol"),
        //        Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString("Telefono"),
        //        FechaIngreso = DateTime.Parse(reader["FechaIngreso"].ToString())
        //    });
        //}

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
        public bool ExisteUsername(string username)
        {
            MySqlCommand cmd = new MySqlCommand(@"
                SELECT COUNT(*) FROM Usuario
                WHERE username =@username
                AND Estado=1");
            cmd.Parameters.AddWithValue("@username", username);
            return Convert.ToInt32(ExecuteScalar(cmd)) > 0;
        }
        public string GetPasswordByUsername(string username)
        {
            MySqlCommand cmd = new MySqlCommand(@"
                SELECT Password 
                FROM Usuario
                WHERE Username = @username
                AND Estado = 1
                LIMIT 1");

            cmd.Parameters.AddWithValue("@username", username);

            using (var reader = ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    return reader["password"].ToString();
                }
            }

            return null; // o string.Empty si prefieres
        }

        public Usuario GetDatosLogin(string username)
        {
            string query = "SELECT Password, Rol,Id FROM Usuario WHERE Username = @username AND Estado = 1 LIMIT 1";
            MySqlCommand command = new MySqlCommand(query);
            command.Parameters.AddWithValue("@username", username);

            using (var reader = ExecuteReader(command))
            {
                if (reader.Read())
                {
                    return new Usuario
                    {
                        Username = username,
                        Password = reader["Password"].ToString(),
                        Rol = reader["Rol"].ToString(),
                        Id = int.Parse(reader["Id"].ToString())
                    };
                }
            }
            return null;
        }
    }
}
