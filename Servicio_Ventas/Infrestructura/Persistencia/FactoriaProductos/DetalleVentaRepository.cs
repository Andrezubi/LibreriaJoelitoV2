using Servicio_Ventas.Aplicacion.Interfaces;
using Servicio_Ventas.Dominio.Modelos;
using MySql.Data.MySqlClient;
using System.Data;

namespace Servicio_Ventas.Infrestructura.Persistencia.FactoriaProductos
{
    public class DetalleVentaRepository : RepositorioBD, IRepository<DetalleVenta>
    {
        public int Insertar(DetalleVenta detalleVenta)
        {
            string consulta = @"INSERT INTO detalleventa ( IdVenta, IdProducto, IdPresentacion, Cantidad, PrecioUnitario, Subtotal)
                                VALUES (@idVenta, @idProducto, @idPresentacion, @cantidad, @precioUnitario, @subtotal);";
            MySqlCommand comando = new MySqlCommand(consulta);

            comando.Parameters.AddWithValue("@idVenta", detalleVenta.IdVenta);
            comando.Parameters.AddWithValue("@idProducto", detalleVenta.IdProducto);
            comando.Parameters.AddWithValue("@idPresentacion", detalleVenta.IdPresentacion);
            comando.Parameters.AddWithValue("@cantidad", detalleVenta.Cantidad);
            comando.Parameters.AddWithValue("@precioUnitario", detalleVenta.PrecioUnitario);
            comando.Parameters.AddWithValue("@subtotal", detalleVenta.Subtotal);
            return RepositorioBD.Instancia.ExecuteNonQuery(comando);
        }

        public int Actualizar(DetalleVenta detalleVenta)
        {
            return 0;
        }

        public DataRow ObtenerPorId(int id)
        {
            return null;
        }

        public DataTable ObtenerTodo()
        {
            string consulta = @"SELECT * 
                                FROM detalleventa";
            MySqlCommand comando = new MySqlCommand(consulta);

            return RepositorioBD.Instancia.ExecuteReturningDataTable(comando);
        }

        public int Eliminar(DetalleVenta detalleVenta)
        {
            string consulta = @"DELETE FROM detalleventa
                                WHERE IdVenta = @idVenta
                                    AND IdProducto = @idProducto
                                    AND IdPresentacion = @idPresentacion";
            MySqlCommand comando = new MySqlCommand(consulta);

            comando.Parameters.AddWithValue("@idVenta", detalleVenta.IdVenta);
            comando.Parameters.AddWithValue("@idProducto", detalleVenta.IdProducto);
            comando.Parameters.AddWithValue("@idPresentacion", detalleVenta.IdPresentacion);

            return RepositorioBD.Instancia.ExecuteNonQuery(comando);
        }

        public DataTable ObtenerPorIdVenta(int idVenta)
        {
            string consulta = @"SELECT dv.IdVenta AS IdVenta, dv.IdProducto AS IdProducto, dv.IdPresentacion AS IdPresentacion, 
                                    dv.Cantidad AS Cantidad, dv.PrecioUnitario AS PrecioUnitario, dv.Subtotal AS Subtotal, 
                                    pp.FactorConversion AS FactorConversion FROM detalleventa dv
                                INNER JOIN presentacionproducto pp ON dv.IdPresentacion = pp.IdPresentacion AND dv.IdProducto = pp.IdProducto
                                WHERE dv.IdVenta = @idVenta";
            MySqlCommand comando = new MySqlCommand(consulta);

            comando.Parameters.AddWithValue("@idVenta", idVenta);

            return RepositorioBD.Instancia.ExecuteReturningDataTable(comando);
        }

        public DataTable ObtenerDetalleExtraPorIdVenta(int idVenta)
        {
            string consulta = @"SELECT dv.IdVenta AS IdVenta, pr.Nombre AS NombreProducto, prs.Nombre AS NombrePresentacion, 
                                    dv.Cantidad AS Cantidad, dv.PrecioUnitario AS PrecioUnitario, dv.Subtotal AS Subtotal, 
                                    pp.FactorConversion AS FactorConversion FROM detalleventa dv
                                INNER JOIN presentacionproducto pp ON dv.IdPresentacion = pp.IdPresentacion AND dv.IdProducto = pp.IdProducto
                                INNER JOIN producto pr ON dv.IdProducto = pr.Id
							    INNER JOIN presentacion prs ON dv.IdPresentacion = prs.Id
                                WHERE dv.IdVenta = @idVenta";
            MySqlCommand comando = new MySqlCommand(consulta);

            comando.Parameters.AddWithValue("@idVenta", idVenta);

            return RepositorioBD.Instancia.ExecuteReturningDataTable(comando);
        }

        public int EliminarPorIdVenta(int idVenta)
        {
            string consulta = @"DELETE FROM detalleventa
                                WHERE IdVenta = @idVenta";
            MySqlCommand comando = new MySqlCommand(consulta);

            comando.Parameters.AddWithValue("@idVenta", idVenta);

            return RepositorioBD.Instancia.ExecuteNonQuery(comando);
        }

        public bool ExisteDuplicado (DetalleVenta detalleVenta)
        {
            return false;
        }
    }
}
