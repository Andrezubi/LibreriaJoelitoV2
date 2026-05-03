using MySql.Data.MySqlClient;
using System.Data;
using System;
using Servicio_Clientes.Dominio.Models;
using Servicio_Clientes.Dominio.Interfaces;

namespace Servicio_Clientes.Infraestructura.Persistencia.FactoryProducts
{
    public class ClienteRepository : ConexionBD, IClienteRepository
    {
        public int Delete(Cliente t)
        {
            MySqlCommand cmd = new MySqlCommand(@"
                UPDATE Cliente SET
                    Estado                   = 0,
                    IdUsuario = @idUsuario,
                    FechaUltimaActualizacion = NOW()
                WHERE Id = @id");
            cmd.Parameters.AddWithValue("idUsuario", t.IdUsuario);
            cmd.Parameters.AddWithValue("id", t.Id);
            return ExecuteNonQuery(cmd);
        }

        public List<Cliente> GetAll()
        {
            MySqlCommand cmd = new MySqlCommand(@"
                SELECT Id, Nombre, ApellidoPaterno, ApellidoMaterno,
                       Ci AS CI, Complemento, Email, ClienteFrecuente AS ClienteFrecuente, FechaRegistro
                FROM Cliente
                WHERE Estado = 1
                ORDER BY ApellidoPaterno, Nombre");

            return ExecuteList(cmd, reader => new Cliente
            {
                Id = reader.GetInt32("Id"),
                Nombre = reader.GetString("Nombre"),
                ApellidoPaterno = reader.GetString("ApellidoPaterno"),
                ApellidoMaterno = reader.IsDBNull(reader.GetOrdinal("ApellidoMaterno")) ? null : reader.GetString("ApellidoMaterno"),
                Ci = reader.GetString("Ci"),
                Complemento = reader.IsDBNull(reader.GetOrdinal("Complemento")) ? null : reader.GetString("Complemento"),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString("Email"),
                ClienteFrecuente = reader.GetBoolean("ClienteFrecuente"),
                FechaRegistro = reader.GetDateTime("FechaRegistro")
            });
        }

        //public List<T> GetById(int id)
        //{
        //    MySqlCommand cmd = new MySqlCommand(@"
        //        SELECT Id, Nombre, ApellidoPaterno, ApellidoMaterno,
        //               Ci AS Ci, Complemento, Email, ClienteFrecuente AS ClienteFrecuente, FechaRegistro
        //        FROM Cliente
        //        WHERE Id = @id AND Estado = 1");

        //    cmd.Parameters.AddWithValue("@id", id);

        //    return ExecuteList(cmd, reader => new Cliente
            //{
            //    Id = reader.GetInt32("Id"),
            //    Nombre = reader.GetString("Nombre"),
            //    ApellidoPaterno = reader.GetString("ApellidoPaterno"),
            //    ApellidoMaterno = reader.IsDBNull(reader.GetOrdinal("ApellidoMaterno")) ? null : reader.GetString("ApellidoMaterno"),
            //    Ci = reader.GetString("Ci"),
            //    Complemento = reader.IsDBNull(reader.GetOrdinal("Complemento")) ? null : reader.GetString("Complemento"),
            //    Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString("Email"),
            //    ClienteFrecuente = reader.GetBoolean("ClienteFrecuente"),
            //    FechaRegistro = reader.GetDateTime("FechaRegistro")
            //});
        //}

        public List<Cliente> GetByCi(string ci)
        {
            MySqlCommand cmd = new MySqlCommand(@"
                SELECT Id, Nombre, ApellidoPaterno, ApellidoMaterno,
                       Ci AS Ci, Complemento, Email, ClienteFrecuente AS ClienteFrecuente, FechaRegistro
                FROM Cliente
                WHERE Ci = @ci AND Estado = 1
                LIMIT 1");

            cmd.Parameters.AddWithValue("@ci", ci);

            return ExecuteList(cmd, reader => new Cliente
            {
                Id = reader.GetInt32("Id"),
                Nombre = reader.GetString("Nombre"),
                ApellidoPaterno = reader.GetString("ApellidoPaterno"),
                ApellidoMaterno = reader.IsDBNull(reader.GetOrdinal("ApellidoMaterno")) ? null : reader.GetString("ApellidoMaterno"),
                Ci = reader.GetString("Ci"),
                Complemento = reader.IsDBNull(reader.GetOrdinal("Complemento")) ? null : reader.GetString("Complemento"),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString("Email"),
                ClienteFrecuente = reader.GetBoolean("ClienteFrecuente"),
                FechaRegistro = reader.GetDateTime("FechaRegistro")
            });
        }

        public int Insert(Cliente t)
        {
            MySqlCommand cmd = new MySqlCommand(@"
                INSERT INTO Cliente 
                    (Nombre, ApellidoPaterno, ApellidoMaterno, Ci, Complemento, Email, ClienteFrecuente, IdUsuario)
                VALUES 
                    (@nombre, @apellidoPaterno, @apellidoMaterno, @ci, @complemento, @email, @clienteFrecuente, @idUsuario);
                SELECT LAST_INSERT_ID();");

            AgregarParametros(cmd, t);

            return Convert.ToInt32(ExecuteScalar(cmd));
        }

        public int Update(Cliente t)
        {
            MySqlCommand cmd = new MySqlCommand(@"
                UPDATE Cliente SET
                    Nombre                   = @nombre,
                    ApellidoPaterno          = @apellidoPaterno,
                    ApellidoMaterno          = @apellidoMaterno,
                    Ci                       = @ci,
                    Complemento              = @complemento,
                    Email                    = @email,
                    ClienteFrecuente         = @clienteFrecuente,
                    IdUsuario                = @idUsuario,
                    FechaUltimaActualizacion = NOW()
                WHERE Id = @id");

            AgregarParametros(cmd, t);
            cmd.Parameters.AddWithValue("@id", t.Id);
            return ExecuteNonQuery(cmd);
        }

        public bool ExisteDuplicado(Cliente cliente)
        {
            MySqlCommand cmd = new MySqlCommand(@"
                SELECT COUNT(*) FROM Cliente
                WHERE Ci          = @ci
                  AND Complemento = @complemento
                  AND Id         <> @id
                  AND Estado      = 1");

            cmd.Parameters.AddWithValue("@ci", cliente.Ci);
            cmd.Parameters.AddWithValue("@complemento", cliente.Complemento ?? string.Empty);
            cmd.Parameters.AddWithValue("@id", cliente.Id);

            return Convert.ToInt32(ExecuteScalar(cmd)) > 0;
        }

        // --- Métodos privados de apoyo ---

        static void AgregarParametros(MySqlCommand cmd, Cliente cliente)
        {
            cmd.Parameters.AddWithValue("@nombre", cliente.Nombre);
            cmd.Parameters.AddWithValue("@apellidoPaterno", cliente.ApellidoPaterno);
            cmd.Parameters.AddWithValue("@apellidoMaterno", (object?)cliente.ApellidoMaterno ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ci", cliente.Ci);
            cmd.Parameters.AddWithValue("@complemento", cliente.Complemento ?? string.Empty);
            cmd.Parameters.AddWithValue("@email", (object?)cliente.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@clienteFrecuente", cliente.ClienteFrecuente);
            cmd.Parameters.AddWithValue("@idUsuario", cliente.IdUsuario);
        }

        public List<Cliente> GetAllSimilarId(string ci)
        {
            MySqlCommand cmd = new MySqlCommand(@"
                SELECT Id, Nombre, ApellidoPaterno, ApellidoMaterno,
                       Ci, Complemento
                FROM Cliente
                WHERE Estado = 1 
                AND CONCAT(Ci, IFNULL(Complemento,'')) LIKE @ci
                LIMIT 10");

            cmd.Parameters.AddWithValue("@ci", "%" + ci + "%");

            return ExecuteList(cmd, reader => new Cliente
            {
                Id = reader.GetInt32("Id"),
                Nombre = reader.GetString("Nombre"),
                ApellidoPaterno = reader.GetString("ApellidoPaterno"),
                ApellidoMaterno = reader.IsDBNull(reader.GetOrdinal("ApellidoMaterno")) ? null : reader.GetString("ApellidoMaterno"),
                Ci = reader.GetString("Ci"),
                Complemento = reader.IsDBNull(reader.GetOrdinal("Complemento")) ? null : reader.GetString("Complemento"),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString("Email"),
                ClienteFrecuente = reader.GetBoolean("ClienteFrecuente"),
                FechaRegistro = reader.GetDateTime("FechaRegistro")
            });
        }

    }
}