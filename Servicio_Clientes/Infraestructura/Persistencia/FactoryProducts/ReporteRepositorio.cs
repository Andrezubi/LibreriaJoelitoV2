using MySql.Data.MySqlClient;
using Servicio_Clientes.Dominio.Models.DTOs;
using Servicio_Clientes.Infraestructura.Persistencia.BD;

namespace Servicio_Clientes.Infraestructura.Persistencia.FactoryProducts
{
    public class ReporteRepositorio : ConexionBD
    {
        public List<ReporteVentaCategoriaDto> ObtenerVentasPorCategoria(
            DateTime fechaDesde, DateTime fechaHasta)
        {
            string query = @"
                SELECT 
                    c.Nombre AS Categoria,
                    SUM(dv.Cantidad) AS TotalUnidades,
                    SUM(dv.Subtotal) AS TotalRecaudado
                FROM DetalleVenta dv
                INNER JOIN producto p      ON dv.IdProducto   = p.Id
                INNER JOIN categoria c     ON p.IdCategoria   = c.Id
                INNER JOIN Venta v         ON dv.IdVenta      = v.Id
                WHERE v.Fecha BETWEEN @fechaDesde AND @fechaHasta
                  AND v.Estado = 1
                GROUP BY c.Id, c.Nombre
                ORDER BY TotalRecaudado DESC";

            MySqlCommand cmd = new MySqlCommand(query);
            cmd.Parameters.AddWithValue("@fechaDesde", fechaDesde.ToString("yyyy-MM-dd 00:00:00"));
            cmd.Parameters.AddWithValue("@fechaHasta", fechaHasta.ToString("yyyy-MM-dd 23:59:59"));

            var result = new List<ReporteVentaCategoriaDto>();
            using var reader = ExecuteReader(cmd);
            while (reader.Read())
            {
                result.Add(new ReporteVentaCategoriaDto
                {
                    Categoria = reader["Categoria"].ToString()!,
                    TotalUnidades = Convert.ToInt32(reader["TotalUnidades"]),
                    TotalRecaudado = Convert.ToDecimal(reader["TotalRecaudado"])
                });
            }
            return result;
        }
    }
}

