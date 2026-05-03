using Servicio_Clientes.Dominio.Models;
using System.Data;

namespace Servicio_Clientes.Dominio.Interfaces
{
    public interface IRepository<T>
    {
        int Insert(T t);
        int Update(T t);
        int Delete(T t);
        List<T> GetAll();
//      List<T> GetById(int id); ya no getById
        bool ExisteDuplicado(T t);
    }
}
