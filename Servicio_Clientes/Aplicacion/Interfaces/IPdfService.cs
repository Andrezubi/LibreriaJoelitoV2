namespace LibreriaJoelitoV2.Aplicacion.Interfaces
{
    public interface IPdfService<T>
    {
        byte[] GenerarComprobanteVenta(List<T> datosVenta);
    }
}