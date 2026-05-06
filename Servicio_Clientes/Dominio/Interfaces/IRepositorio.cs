using System.Data;

namespace Servicio_Clientes.Dominio.Interfaces
{
    public interface IRepositorio<T>
    {
        int Insertar(T t);
        int Actualizar(T t);
        int Eliminar(T t);
        DataTable ObtenerTodo();
        DataRow? ObtenerPorId(int id);
        bool ExisteDuplicado(T t);
    }
}
