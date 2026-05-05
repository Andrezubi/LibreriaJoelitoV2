using Servicio_Ventas.Aplicacion.Interfaces;
using Servicio_Ventas.Dominio.Modelos;
using MySql.Data.MySqlClient;
using System.Data;

namespace Servicio_Ventas.Infrestructura.Persistencia.FactoriaProductos
{
    public class VentaRepository : RepositorioBD, IRepositorio<Venta>
    {
        public int Insertar(Venta venta)
        {
            string consulta = @"INSERT INTO venta (IdCliente,Total,IdUsuario)
                                VALUES (@idCliente,@total,@idUsuario);
                                SELECT LAST_INSERT_ID();";
            MySqlCommand comando = new MySqlCommand(consulta);

            comando.Parameters.AddWithValue("@idCliente", venta.IdCliente);
            comando.Parameters.AddWithValue("@total", venta.Total);
            comando.Parameters.AddWithValue("@idUsuario", venta.IdUsuario);

            return Convert.ToInt32(ExecuteScalar(comando));
        }

        public int Eliminar(Venta venta)
        {
            string consulta = @"UPDATE venta
                                SET Estado = 0, FechaUltimaActualizacion=@fechaAhora, IdUsuario=@idUsuario
                                WHERE Id = @Id";
            MySqlCommand comando = new MySqlCommand(consulta);

            comando.Parameters.AddWithValue("@fechaAhora", DateTime.Now);
            comando.Parameters.AddWithValue("@idUsuario", venta.IdUsuario);
            comando.Parameters.AddWithValue("@Id", venta.Id);

            return ExecuteNonQuery(comando);
        }

        public List<Venta> ObtenerTodo()
        {
            string consulta = @"SELECT  Id, IdCliente, Fecha, Total, FechaRegistro, IdUsuario
                                FROM venta
                                WHERE Estado=1
                                ORDER BY 3";
            MySqlCommand comando = new MySqlCommand(consulta);

            var result = new List<Venta>();
            var reader = ExecuteReader(comando);

            while (reader.Read())
            {
                result.Add(new Venta
                {
                    Id = reader.GetInt32("Id"),
                    IdCliente = reader.GetInt32("IdCliente"),
                    Fecha = reader.GetDateTime("Fecha"),
                    Total = reader.GetDecimal("Total"),
                    FechaRegistro = reader.GetDateTime("FechaRegistro"),
                    IdUsuario = reader.GetInt32("IdUsuario")
                });
            }

            return result;
        }

        public DataRow ObtenerPorId(int id)
        {
            string consulta = @"SELECT  Id, IdCliente, Fecha, Total, FechaRegistro, FechaUltimaActualizacion, IdUsuario
                                FROM venta
                                WHERE Estado=1 and Id=@id
                                ORDER BY 3";

            MySqlCommand comando = new MySqlCommand(consulta);
            comando.Parameters.AddWithValue("@id", id);

            return ExecuteReturningDataRow(comando);
        }

        public DataRow ObtenerCabeceraVentaPorId(int id)
        {
            string consulta = @"SELECT v.Id,
                                   v.Estado AS EstadoVenta,
                                   c.Ci AS CiCliente,
                                   c.Nombre AS NombreCliente,
                                   u.Nombre AS NombreEmpleado,
                                   v.Fecha,
                                   v.Total
                                FROM venta v
                                INNER JOIN cliente c ON v.IdCliente = c.Id
                                INNER JOIN usuario u ON v.IdUsuario = u.Id
                                WHERE v.Estado=1 and v.Id=@id
                                ORDER BY v.Fecha DESC";

            MySqlCommand comando = new MySqlCommand(consulta);
            comando.Parameters.AddWithValue("@id", id);

            return ExecuteReturningDataRow(comando);
        }

        public DataTable ObtenerPorFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            string consulta = @"SELECT  Id, IdCliente, Fecha, Total, FechaRegistro, IdUsuario
                                FROM venta
                                WHERE Estado=1
                                    AND Fecha BETWEEN @fechaInicio AND @fechaFin
                                ORDER BY 3";
            MySqlCommand comando = new MySqlCommand(consulta);

            comando.Parameters.AddWithValue("@fechaInicio", fechaInicio);
            comando.Parameters.AddWithValue("@fechaFin", fechaFin);

            return ExecuteReturningDataTable(comando);
        }

        public DataTable ObtenerPorIdCliente(int idCliente)
        {
            string consulta = @"SELECT  Id, IdCliente, Fecha, Total, FechaRegistro, IdUsuario
                                FROM venta
                                WHERE Estado=1 
                                    AND IdCliente=@idCliente
                                ORDER BY 3";
            MySqlCommand comando = new MySqlCommand(consulta);

            comando.Parameters.AddWithValue("@idCliente", idCliente);

            return ExecuteReturningDataTable(comando);
        }

        public int Actualizar(Venta venta)
        {
            string consulta = @"UPDATE venta
                                SET IdCliente = @idCliente,
                                    Fecha = @fecha,
                                    Total = @total,
                                    FechaUltimaActualizacion=@fechaAhora,
                                    IdUsuario=@idUsuario
                                WHERE Id = @Id";
            MySqlCommand comando = new MySqlCommand(consulta);

            comando.Parameters.AddWithValue("@idCliente", venta.IdCliente);
            comando.Parameters.AddWithValue("@fecha", venta.Fecha);
            comando.Parameters.AddWithValue("@total", venta.Total);
            comando.Parameters.AddWithValue("@idUsuario", venta.IdUsuario);
            comando.Parameters.AddWithValue("@fechaAhora", DateTime.Now);
            comando.Parameters.AddWithValue("@Id", venta.Id);

            return ExecuteNonQuery(comando);
        }

        public bool ExisteDuplicado(Venta venta)
        {
            return false;
        }

        public DataTable ObtenerDatosComprobante(int idVenta)
        {
            string consulta = @"SELECT 
                                    v.Id AS VentaId, 
                                    v.Fecha, 
                                    v.Total,
                                    v.FechaRegistro,
                                    c.Ci, 
                                    c.Complemento, 
                                    c.Nombre AS ClienteNombre, 
                                    c.ApellidoPaterno, 
                                    c.ApellidoMaterno,
                                    u.Username AS NombreEmpleado,
                                    dv.Cantidad, 
                                    CONCAT(pr.Nombre, ' de ', p.Nombre, ' ', m.Nombre) AS DescripcionProducto,
                                    dv.PrecioUnitario, 
                                    dv.Subtotal
                                FROM venta v
                                INNER JOIN cliente c ON v.IdCliente = c.Id
                                INNER JOIN usuario u ON v.IdUsuario = u.Id
                                INNER JOIN detalleventa dv ON v.Id = dv.IdVenta
                                INNER JOIN producto p ON dv.IdProducto = p.Id
                                INNER JOIN marca m ON p.IdMarca = m.Id
                                INNER JOIN presentacion pr ON dv.IdPresentacion = pr.Id
                                WHERE v.Id = @idVenta AND v.Estado = 1";

            MySqlCommand comando = new MySqlCommand(consulta);
            comando.Parameters.AddWithValue("@idVenta", idVenta);

            return ExecuteReturningDataTable(comando);
        }

        public DataTable CargarVentas()
        {
            string consulta = @"SELECT v.Id,
                                    v.Estado AS EstadoVenta,
                                    c.Ci AS CiCliente,
                                    c.Nombre AS NombreCliente,
                                    v.Fecha
                                FROM venta v
                                INNER JOIN cliente c ON v.IdCliente = c.Id
                                INNER JOIN usuario u ON v.IdUsuario = u.Id
                                ORDER BY v.Fecha DESC";

            MySqlCommand comando = new MySqlCommand(consulta);

            return ExecuteReturningDataTable(comando);
        }
    }
}
