
using MySql.Data.MySqlClient;
using Servicio_Ventas.Aplicacion.Interfaces;
using Servicio_Ventas.Dominio.Modelos;
using System.Data;

namespace Servicio_Ventas.Infrestructura.Persistencia.FactoriaProductos
{
    public class PresentacionProductoRepositorio : RepositorioBD, IRepositorio<PresentacionProducto>
    {
        public int Actualizar(PresentacionProducto t)
        {
            throw new NotImplementedException();
        }

        public int Eliminar(PresentacionProducto t)
        {
            throw new NotImplementedException();
        }

        public int Insertar(PresentacionProducto t)
        {
            throw new NotImplementedException();
        }

        public List<PresentacionProducto> ObtenerTodo()
        {
            throw new NotImplementedException();
        }


        public PresentacionProducto? ObtenerPorIds(int idProducto, int idPresentacion)
        {
            string query = @"
                                SELECT 
                                    pp.IdProducto,
                                    pp.IdPresentacion,
                                    pp.Precio,
                                    pp.FactorConversion AS FactSorConversion,
                                    p.Nombre AS Producto,
                                    pr.Nombre AS Presentacion,
                                    m.Nombre AS Marca,
                                    CONCAT(pr.Nombre, ' de ', p.Nombre, ' ', m.Nombre) AS Descripcion
                                FROM PresentacionProducto pp
                                INNER JOIN Producto p ON pp.IdProducto = p.Id
                                INNER JOIN Presentacion pr ON pp.IdPresentacion = pr.Id
                                LEFT JOIN Marca m ON p.IdMarca = m.Id
                                WHERE pp.IdProducto = @idProducto
                                  AND pp.IdPresentacion = @idPresentacion
                                  AND pp.Estado = 1
                                  AND p.Estado = 1
                                  AND pr.Estado = 1";

            var cmd = new MySqlCommand(query);

            cmd.Parameters.AddWithValue("@idProducto", idProducto);
            cmd.Parameters.AddWithValue("@idPresentacion", idPresentacion);

            var dt = ExecuteReturningDataTable(cmd);

            if (dt.Rows.Count > 0)
                return dt.Rows[0];

            return null;
        }

        public DataTable obtenerPresentacionProductoDetallado(string frase)
        {
            string query = @"SELECT 
                                pp.IdProducto,
                                pp.IdPresentacion,
                                pp.Estado AS EstadoPresentacionProducto,
                                p.Nombre AS Producto,
                                pr.Nombre AS Presentacion,
                                m.Nombre AS Marca,
                                CONCAT(pr.Nombre, ' de ', p.Nombre, ' ', m.Nombre) AS Descripcion,
                                pp.Precio
                            FROM PresentacionProducto pp
                            INNER JOIN Producto p 
                                ON pp.IdProducto = p.Id
                            INNER JOIN Presentacion pr 
                                ON pp.IdPresentacion = pr.Id
                            LEFT JOIN Marca m 
                                ON p.IdMarca = m.Id
                            WHERE CONCAT(pr.Nombre, ' de ', p.Nombre, ' ', m.Nombre) 
                                  LIKE CONCAT('%', @frase, '%')
                              AND pp.Estado = TRUE
                              AND p.Estado = TRUE
                              AND pr.Estado = TRUE
                              AND (m.Estado = TRUE OR m.Id IS NULL);";
            MySqlCommand cmd = new MySqlCommand(query);
            cmd.Parameters.AddWithValue("@frase", frase);
            return ExecuteReturningDataTable(cmd);

        }


        public int InsertarRelacion(int idProducto, int idPresentacion, double factorConversion, decimal precio, int? idUsuario)
        {
            string query = @"INSERT INTO presentacionproducto (IdProducto, IdPresentacion, FactorConversion, Precio, IdUsuario) 
                     VALUES (@idProd, @idPres, @factor, @precio, @idUsu)";

            MySqlCommand cmd = new MySqlCommand(query);
            cmd.Parameters.AddWithValue("@idProd", idProducto);
            cmd.Parameters.AddWithValue("@idPres", idPresentacion);
            cmd.Parameters.AddWithValue("@factor", factorConversion);
            cmd.Parameters.AddWithValue("@precio", precio);
            cmd.Parameters.AddWithValue("@idUsu", idUsuario);

            return ExecuteNonQuery(cmd);
        }

    }

        

        
    }
}
