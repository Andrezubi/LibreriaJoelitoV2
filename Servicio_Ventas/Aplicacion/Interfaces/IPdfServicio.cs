namespace Servicio_Ventas.Aplicacion.Interfaces
{
    public interface IPdfServicio
    {
        byte[] GenerarComprobanteVenta(System.Data.DataTable datosVenta);
    }
}