using Servicio_Ventas.Aplicacion.Interfaces;
using Servicio_Ventas.Aplicacion.Results;
using Servicio_Ventas.Dominio.Modelos;
using Servicio_Ventas.Infrestructura.Persistencia;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Servicio_Ventas.Infrestructura.Persistencia.FactoriaProductos;

namespace Servicio_Ventas.Aplicacion.Servicios
{
    public class VentaService 
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

                var clienteFila = _clienteRepositorio.ObtenerPorId(venta.IdCliente);
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
                            throw new Exception($"Error al insertar el detalle para el producto: {_productoRepositorio.ObtenerPorId(detalle.IdProducto)?["Nombre"]}");

                        // --- NUEVA LÓGICA DE FACTOR DE CONVERSIÓN ---

                        // A) Consultamos la presentación a la base de datos para obtener el factor de forma segura
                        DataRow presentacionFila = _presentaProdRepositorio.GetByIds(detalle.IdProducto, detalle.IdPresentacion);
                        if (presentacionFila == null)
                            throw new Exception("No se encontró la presentación del producto especificado.");

                        int factorConversion = Convert.ToInt32(presentacionFila["FactorConversion"]);

                        // B) Calculamos la cantidad real a descontar del inventario general (unidades)
                        int cantidadRealADescontar = detalle.Cantidad * factorConversion;

                        // C) Descontamos el stock usando la cantidad real multiplicada
                        int filasStock = _productoRepositorio.DescontarStock(detalle.IdProducto, cantidadRealADescontar);
                        if (filasStock <= 0)
                        {
                            // Si no afectó filas es porque el Stock < CantidadReal (validación lógica en el SQL)
                            throw new Exception($"Stock insuficiente para el producto: {_productoRepositorio.ObtenerPorId(detalle.IdProducto)?["Nombre"]}");
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
                    DataTable detallesDt = _detalleVentaRepositorio.ObtenerPorIdVenta(Convert.ToInt32(idVenta));

                    foreach (DataRow fila in detallesDt.Rows)
                    {
                        int idProducto = Convert.ToInt32(fila["IdProducto"]);
                        int cantidad = Convert.ToInt32(fila["Cantidad"]) * Convert.ToInt32(fila["FactorConversion"]);

                        int filasStock = _productoRepositorio.RestaurarStock(idProducto, cantidad);
                        if (filasStock <= 0)
                            return Result<int>.Failure($"Error al restaurar el stock del producto ID {idProducto}.");
                    }

                    Venta venta = new Venta
                    {
                        Id = idVenta,
                        IdUsuario = idEmpleado
                    };

                    int resultado = _ventaRepositorio.Eliminar(venta);
                    if (resultado <= 0)
                        return Result<int>.Failure("No se pudo actualizar el estado de la venta.");

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
            DataRow fila = _presentaProdRepositorio.GetByIds(idProducto, idPresentacion);

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
    }
}
