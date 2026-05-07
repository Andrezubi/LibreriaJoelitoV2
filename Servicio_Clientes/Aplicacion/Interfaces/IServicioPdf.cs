namespace Servicio_Clientes.Aplicacion.Interfaces
{
    public interface IServicioPdf
    {
        byte[] GenerarReporteUsuariosPdf(System.Data.DataTable usuarios);
    }
}