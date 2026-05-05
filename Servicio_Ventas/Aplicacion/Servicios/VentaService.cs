using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Servicio_Ventas.Aplicacion.DTOs;
using Servicio_Ventas.Aplicacion.Interfaces;
using Servicio_Ventas.Aplicacion.Results;
using Servicio_Ventas.Dominio.Modelos;
using Servicio_Ventas.Infrestructura.Persistencia;
using Servicio_Ventas.Infrestructura.Persistencia.FactoriaProductos;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Servicio_Ventas.Aplicacion.Servicios
{
    public class VentaService : RepositorioBD
    {
        private readonly VentaRepository _ventaRepositorio;
        private readonly DetalleVentaRepository _detalleVentaRepositorio;
        //private readonly IRepository<Producto> _productoRepositorio;
        //private readonly IRepository<Cliente> _clienteRepositorio;
        //private readonly IRepository<PresentacionProducto> _presentaProdRepositorio;
        private readonly IPdfServicio _pdfServicio;
        public VentaService(
            //IRepository<PresentacionProducto> presentProdRepositorio,
            VentaRepository ventaRepositorio,
            DetalleVentaRepository detalleVentaRepositorio,
            //IRepository<Producto> productoRepositorio,
            IPdfServicio pdfServicio
            //IRepository<Cliente> clienteRepositorio
        )
        {
            _ventaRepositorio = ventaRepositorio;
            _detalleVentaRepositorio = detalleVentaRepositorio;
            //_productoRepositorio = productoRepositorio;
            //_clienteRepositorio = clienteRepositorio;
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

        public Result<int> RegistrarVenta(Venta venta, List<DetalleVenta> detalles)
        {
            try
            {
                // 1. Validaciones previas (Fuera de transacción para no bloquear)
                if (detalles == null || !detalles.Any())
                    return Result<int>.Failure("La venta debe tener al menos un producto.");

                //var clienteFila = _clienteRepositorio.ObtenerPorId(venta.IdCliente);
                var clienteFila = ObtenerPorId(venta.IdCliente);
                if (clienteFila == null)
                    return Result<int>.Failure("El cliente seleccionado no es válido.");

                // 2. Iniciar Proceso Atómico
                RepositorioBD.Instancia.BeginTransaction();

                try
                {
                    // 3. Insertar Cabecera de Venta
                    int ventaId = _ventaRepositorio.Insertar(venta);
                    if (ventaId <= 0)
                        throw new Exception("No se pudo generar la cabecera de la venta.");

                    // 4. Procesar Detalles y Stock
                    foreach (var detalle in detalles)
                    {
                        detalle.IdVenta = ventaId;

                        // Insertar Detalle
                        int filasDetalle = _detalleVentaRepositorio.Insertar(detalle);
                        if (filasDetalle <= 0)
                            //throw new Exception($"Error al insertar el detalle para el producto: {_productoRepositorio.ObtenerPorId(detalle.IdProducto)?["Nombre"]}");
                            throw new Exception($"Error al insertar el detalle para el producto: {ObtenerPorIdP(detalle.IdProducto)?["Nombre"]}");

                        // --- NUEVA LÓGICA DE FACTOR DE CONVERSIÓN ---

                        // A) Consultamos la presentación a la base de datos para obtener el factor de forma segura
                        //DataRow presentacionFila = _presentaProdRepositorio.GetByIds(detalle.IdProducto, detalle.IdPresentacion);
                        DataRow presentacionFila = GetByIds(detalle.IdProducto, detalle.IdPresentacion);
                        if (presentacionFila == null)
                            throw new Exception("No se encontró la presentación del producto especificado.");

                        int factorConversion = Convert.ToInt32(presentacionFila["FactorConversion"]);

                        // B) Calculamos la cantidad real a descontar del inventario general (unidades)
                        int cantidadRealADescontar = detalle.Cantidad * factorConversion;

                        // C) Descontamos el stock usando la cantidad real multiplicada
                        //int filasStock = _productoRepositorio.DescontarStock(detalle.IdProducto, cantidadRealADescontar);
                        int filasStock = DescontarStock(detalle.IdProducto, cantidadRealADescontar);
                        if (filasStock <= 0)
                        {
                            // Si no afectó filas es porque el Stock < CantidadReal (validación lógica en el SQL)
                            //throw new Exception($"Stock insuficiente para el producto: {_productoRepositorio.ObtenerPorId(detalle.IdProducto)?["Nombre"]}");
                            throw new Exception($"Stock insuficiente para el producto: {ObtenerPorIdP(detalle.IdProducto)?["Nombre"]}");
                        }
                    }

                    // 5. Confirmar todo
                    RepositorioBD.Instancia.Commit();
                    return Result<int>.Success(ventaId);
                }
                catch (Exception ex)
                {
                    // 6. Revertir si algo falló
                    RepositorioBD.Instancia.Rollback();
                    return Result<int>.Failure($"Error en la transacción: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return Result<int>.Failure($"Error inesperado: {ex.Message}");
            }
        }

        public Result<int> AnularVenta(int idVenta, int idEmpleado)
        {
            try
            {
                var ventaFila = _ventaRepositorio.ObtenerPorId(idVenta);

                if (ventaFila == null)
                    return Result<int>.Failure("La venta ya ha sido anulada antes.");

                RepositorioBD.Instancia.BeginTransaction();

                try
                {
                    List<DetalleVentaStockDTO> detalles = _detalleVentaRepositorio.ObtenerPorIdVenta(idVenta);

                    if (detalles == null || detalles.Count == 0)
                    {
                        RepositorioBD.Instancia.Rollback();
                        return Result<int>.Failure("No se encontraron detalles para la venta.");
                    }

                    foreach (DetalleVentaStockDTO detalle in detalles)
                    {
                        int idProducto = detalle.IdProducto;
                        int cantidad = Convert.ToInt32(detalle.Cantidad * detalle.FactorConversion);

                        int filasStock = RestaurarStock(idProducto, cantidad);

                        if (filasStock <= 0)
                        {
                            RepositorioBD.Instancia.Rollback();
                            return Result<int>.Failure($"Error al restaurar el stock del producto ID {idProducto}.");
                        }
                    }

                    Venta venta = new Venta
                    {
                        Id = idVenta,
                        IdUsuario = idEmpleado
                    };

                    int resultado = _ventaRepositorio.Eliminar(venta);

                    if (resultado <= 0)
                    {
                        RepositorioBD.Instancia.Rollback();
                        return Result<int>.Failure("No se pudo actualizar el estado de la venta.");
                    }

                    RepositorioBD.Instancia.Commit();

                    return Result<int>.Success(venta.Id);
                }
                catch (Exception ex)
                {
                    RepositorioBD.Instancia.Rollback();
                    return Result<int>.Failure($"Transacción revertida. Error: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return Result<int>.Failure($"Error inesperado al anular: {ex.Message}");
            }
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
                        idPresentacion=idPresentacion,
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

        // TODO: Mover a ClienteRepository
        public DataRow ObtenerPorId(int id)
        {
            MySqlCommand cmd = new MySqlCommand(@"
                SELECT Id, Nombre, ApellidoPaterno, ApellidoMaterno,
                       Ci AS Ci, Complemento, Email, ClienteFrecuente AS ClienteFrecuente, FechaRegistro
                FROM Cliente
                WHERE Id = @id AND Estado = 1");

            cmd.Parameters.AddWithValue("@id", id);

            return ExecuteReturningDataRow(cmd);
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

            var dt = ExecuteReturningDataTable(cmd);

            if (dt.Rows.Count > 0)
                return dt.Rows[0];

            return null;
        }

        // TODO: Mover a ProductoRepository
        public DataRow ObtenerPorIdP(int id)
        {
            string query = @"SELECT  Id, Nombre,IdCategoria,IdMarca,Stock,Estado,FechaRegistro,IdUsuario,FechaUltimaActualizacion
                            FROM producto
                            WHERE Estado=1 and Id=@id
                            ORDER BY 3";

            MySqlCommand command = new MySqlCommand(query);
            command.Parameters.AddWithValue("@id", id);

            return ExecuteReturningDataRow(command);
        }

        public int DescontarStock(int idProducto, int cantidad)
        {
            string query = @"UPDATE producto 
                             SET Stock = Stock - @cantidad, 
                                 FechaUltimaActualizacion = @fechaAhora 
                             WHERE Id = @idProducto AND Stock >= @cantidad;";

            MySqlCommand command = new MySqlCommand(query);
            command.Parameters.AddWithValue("@cantidad", cantidad);
            command.Parameters.AddWithValue("@idProducto", idProducto);
            command.Parameters.AddWithValue("@fechaAhora", DateTime.Now);

            return ExecuteNonQuery(command);
        }

        public int RestaurarStock(int idProducto, int cantidad)
        {
            string query = @"UPDATE producto 
                             SET Stock = Stock + @cantidad, 
                                 FechaUltimaActualizacion = @fechaAhora 
                             WHERE Id = @idProducto;";

            MySqlCommand command = new MySqlCommand(query);
            command.Parameters.AddWithValue("@cantidad", cantidad);
            command.Parameters.AddWithValue("@idProducto", idProducto);
            command.Parameters.AddWithValue("@fechaAhora", DateTime.Now);
            return ExecuteNonQuery(command);
        }
    }
}
