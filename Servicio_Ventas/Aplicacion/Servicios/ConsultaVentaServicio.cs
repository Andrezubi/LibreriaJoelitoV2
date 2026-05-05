using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Servicio_Ventas.Aplicacion.Interfaces;
using Servicio_Ventas.Aplicacion.Results;
using Servicio_Ventas.Infrestructura.Persistencia;
using Servicio_Ventas.Infrestructura.Persistencia.FactoriaProductos;
using System.Data;

namespace Servicio_Ventas.Aplicacion.Servicios
{
    public class ConsultaVentaServicio
    {
        private readonly VentaRepositorio _ventaRepositorio;
        private readonly DetalleVentaRepositorio _detalleVentaRepositorio;
        //private readonly IRepository<PresentacionProducto> _presentaProdRepositorio;
        private readonly IPdfServicio _pdfServicio;

        public ConsultaVentaServicio(
            //IRepository<PresentacionProducto> presentProdRepositorio,
            VentaRepositorio ventaRepositorio,
            DetalleVentaRepositorio detalleVentaRepositorio,
            IPdfServicio pdfServicio
        )
        {
            _ventaRepositorio = ventaRepositorio;
            _detalleVentaRepositorio = detalleVentaRepositorio;
            //_presentaProdRepositorio = presentProdRepositorio;
            _pdfServicio = pdfServicio;
        }

        public DataTable getPresentacionProductosByFrase(string frase)
        {
            //return _presentaProdRepositorio.obtenerPresentacionProductoDetallado(frase);
            throw new NotImplementedException();
        }

        public DataTable CargarVentas()
        {
            return _ventaRepositorio.CargarVentas();
        }

        public JsonResult getPresentacionProductoByIds(int idProducto, int idPresentacion)
        {
            //DataRow fila = _presentaProdRepositorio.GetByIds(idProducto, idPresentacion);
            DataRow fila = GetByIds(idProducto, idPresentacion);

            if (fila != null)
            {
                return new JsonResult(new
                {
                    success = true,
                    producto = new
                    {
                        idProducto = idProducto,
                        idPresentacion = idPresentacion,
                        nombre = fila["Descripcion"].ToString(),
                        precioUnitario = Convert.ToDecimal(fila["Precio"])
                    }
                });
            }

            return new JsonResult(new { success = false });
        }

        public Result<byte[]> GenerarComprobantePdf(int idVenta)
        {
            try
            {
                // 1. Pedimos los datos al repositorio (La consulta de los Joins)
                DataTable dt = _ventaRepositorio.ObtenerDatosComprobante(idVenta);

                if (dt == null || dt.Rows.Count == 0)
                    return Result<byte[]>.Failure("No se encontró la venta.");

                // 2. Delegamos la creación del archivo al servicio especializado
                byte[] pdf = _pdfServicio.GenerarComprobanteVenta(dt);

                return Result<byte[]>.Success(pdf);
            }
            catch (Exception ex)
            {
                return Result<byte[]>.Failure($"Error en fachada de PDF: {ex.Message}");
            }
        }

        public (DataRow venta, DataTable detalles) ObtenerVentaCompleta(int idVenta)
        {
            var ventaFila = _ventaRepositorio.ObtenerCabeceraVentaPorId(idVenta);
            if (ventaFila == null)
                throw new Exception("No se encontró la venta.");

            var detalles = _detalleVentaRepositorio.ObtenerDetalleExtraPorIdVenta(idVenta);

            return (ventaFila, detalles);
        }

        // TODO: Mover a PresentacionProductoRepository
        public DataRow? GetByIds(int idProducto, int idPresentacion)
        {
            string query = @"
                                SELECT 
                                    pp.IdProducto,
                                    pp.IdPresentacion,
                                    pp.Precio,
                                    pp.FactorConversion AS FactorConversion,
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

            var dt = RepositorioBD.Instancia.ExecuteReturningDataTable(cmd);

            if (dt.Rows.Count > 0)
                return dt.Rows[0];

            return null;
        }
    }
}