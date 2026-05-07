namespace Servicio_Clientes.Aplicacion.Interfaces
{
    public interface IServicioEmail
    {
        Task EnviarCorreoAsync(string para, string asunto, string cuerpo);
    }
}
