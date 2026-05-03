using LibreriaJoelito.Dominio.Models;
using System.Data;

namespace LibreriaJoelito.Aplicacion.Interfaces
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
