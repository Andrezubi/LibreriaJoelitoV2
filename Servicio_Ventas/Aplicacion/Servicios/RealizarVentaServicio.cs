using MySql.Data.MySqlClient;
using Servicio_Ventas.Aplicacion.Interfaces;
using Servicio_Ventas.Aplicacion.Results;
using Servicio_Ventas.Dominio.Modelos;
using Servicio_Ventas.Infrestructura.Persistencia;
using Servicio_Ventas.Infrestructura.Persistencia.FactoriaProductos;
using System.Data;

namespace Servicio_Ventas.Aplicacion.Servicios
{
    public class RealizarVentaServicio 
    {
        private readonly VentaRepositorio _ventaRepositorio;
        private readonly DetalleVentaRepositorio _detalleVentaRepositorio;
        //private readonly IRepository<Producto> _productoRepositorio;
        //private readonly IRepository<Cliente> _clienteRepositorio;
        //private readonly IRepository<PresentacionProducto> _presentaProdRepositorio;

        public RealizarVentaServicio(
            //IRepository<PresentacionProducto> presentProdRepositorio,
            VentaRepositorio ventaRepositorio,
            DetalleVentaRepositorio detalleVentaRepositorio
        //IRepository<Producto> productoRepositorio,
        //IRepository<Cliente> clienteRepositorio
        )
        {
            _ventaRepositorio = ventaRepositorio;
            _detalleVentaRepositorio = detalleVentaRepositorio;
            //_productoRepositorio = productoRepositorio;
            //_clienteRepositorio = clienteRepositorio;
            //_presentaProdRepositorio = presentProdRepositorio;
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

        // TODO: Mover a ClienteRepository
        public DataRow ObtenerPorId(int id)
        {
            MySqlCommand cmd = new MySqlCommand(@"
                SELECT Id, Nombre, ApellidoPaterno, ApellidoMaterno,
                       Ci AS Ci, Complemento, Email, ClienteFrecuente AS ClienteFrecuente, FechaRegistro
                FROM Cliente
                WHERE Id = @id AND Estado = 1");

            cmd.Parameters.AddWithValue("@id", id);

            return RepositorioBD.Instancia.ExecuteReturningDataRow(cmd);
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

        // TODO: Mover a ProductoRepository
        public DataRow ObtenerPorIdP(int id)
        {
            string query = @"SELECT  Id, Nombre,IdCategoria,IdMarca,Stock,Estado,FechaRegistro,IdUsuario,FechaUltimaActualizacion
                            FROM producto
                            WHERE Estado=1 and Id=@id
                            ORDER BY 3";

            MySqlCommand command = new MySqlCommand(query);
            command.Parameters.AddWithValue("@id", id);

            return RepositorioBD.Instancia.ExecuteReturningDataRow(command);
        }
            
        // TODO: Mover a ProductoRepository
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

            return RepositorioBD.Instancia.ExecuteNonQuery(command);
        }
    }
}