using System.Data;

namespace Servicio_Ventas.Aplicacion.Interfaces
{
    public interface IRepositorio<T>
    {
        int Insertar(T t);
        int Actualizar(T t);
        int Eliminar(T t);
        List<T> ObtenerTodo();
    }
}
