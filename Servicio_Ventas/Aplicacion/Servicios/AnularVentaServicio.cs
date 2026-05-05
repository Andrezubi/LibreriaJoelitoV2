using MySql.Data.MySqlClient;
using Servicio_Ventas.Aplicacion.DTOs;
using Servicio_Ventas.Aplicacion.Results;
using Servicio_Ventas.Dominio.Modelos;
using Servicio_Ventas.Infrestructura.Persistencia;
using Servicio_Ventas.Infrestructura.Persistencia.FactoriaProductos;

namespace Servicio_Ventas.Aplicacion.Servicios
{
    public class AnularVentaServicio 
    {
        private readonly VentaRepositorio _ventaRepositorio;
        private readonly DetalleVentaRepositorio _detalleVentaRepositorio;

        public AnularVentaServicio(
            VentaRepositorio ventaRepositorio,
            DetalleVentaRepositorio detalleVentaRepositorio)
        {
            _ventaRepositorio = ventaRepositorio;
            _detalleVentaRepositorio = detalleVentaRepositorio;
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


        // TODO: Mover a ProductoRepository
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
            return RepositorioBD.Instancia.ExecuteNonQuery(command);
        }
    }
}