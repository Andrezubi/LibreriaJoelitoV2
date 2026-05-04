using System.Data;

namespace Servicio_Ventas.Aplicacion.Interfaces
{
    public interface IRepository<T>
    {
        int Insertar(T t);
        int Actualizar(T t);
        int Eliminar(T t);
        DataTable ObtenerTodo();
        DataRow? ObtenerPorId(int id);
        bool ExisteDuplicado(T t);
    }
}
