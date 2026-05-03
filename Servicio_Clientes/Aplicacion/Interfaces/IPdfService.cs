namespace LibreriaJoelitoV2.Aplicacion.Interfaces
{
    public interface IPdfService
    {
        byte[] GenerarComprobanteVenta(System.Data.DataTable datosVenta);
    }
}