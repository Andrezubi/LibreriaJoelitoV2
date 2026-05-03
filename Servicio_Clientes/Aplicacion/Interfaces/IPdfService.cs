namespace Servicio_Clientes.Aplicacion.Interfaces
{
    public interface IPdfService
    {
        byte[] GenerarComprobanteVenta<T>(List<T> datosVenta);
    }
}