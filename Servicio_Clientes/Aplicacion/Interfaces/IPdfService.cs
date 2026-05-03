namespace Servicio_Clientes.Aplicacion.Interfaces
{
    public interface IPdfService<T>
    {
        byte[] GenerarComprobanteVenta(List<T> datosVenta);
    }
}