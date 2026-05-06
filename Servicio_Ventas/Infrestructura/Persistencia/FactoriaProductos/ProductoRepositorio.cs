
using MySql.Data.MySqlClient;
using Servicio_Ventas.Aplicacion.Interfaces;
using Servicio_Ventas.Dominio.Modelos;
using Servicio_Ventas.Infrestructura.Persistencia;
using System.Configuration;
using System.Data;
using System.Reflection.PortableExecutable;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LibreriaJoelito.Infraestructura.Persistencia.FactoryProducts
{
    public class ProductoRepositorio : RepositorioBD, IRepositorio<Producto>
    {
        public int DescontarStock(int idProducto, int cantidad)
        {
            string query = @"UPDATE producto 
                             SET Stock = Stock - @cantidad, 
                                 FechaUltimaActualizacion = @fechaAhora 
                             WHERE Id = @idProducto AND Stock >= @cantidad;";
            
            MySqlCommand command = new MySqlCommand(query);
            command.Parameters.AddWithValue("@cantidad", cantidad);
            command.Parameters.AddWithValue("@idProducto", idProducto);
            command.Parameters.AddWithValue("@fechaAhora", DateTime.Now);

            return ExecuteNonQuery(command);
        }
        

        public Producto ObtenerPorId(int id)
        {
            string query = @"SELECT  Id, Nombre,IdCategoria,IdMarca,Stock,Estado,FechaRegistro,IdUsuario,FechaUltimaActualizacion
                            FROM producto
                            WHERE Estado=1 and Id=@id
                            ORDER BY 3";
            
            MySqlCommand command = new MySqlCommand(query);
            command.Parameters.AddWithValue("@id", id);
            Producto result= new Producto();
            var reader = ExecuteReader(command);
;            while (reader.Read())
            {

                result =
                    new Producto
                    {
                        Id = reader.GetInt32("Id"),
                        Nombre = reader["Nombre"].ToString(),
                        IdCategoria = reader.GetInt32("IdCategoria"),
                        IdMarca = reader.GetInt32("IdMarca"),
                        Stock = reader.GetInt32("Stock"),
                        Estado = (bool)reader["Estado"],
                        FechaRegistro = (DateTime)reader["FechaRegistro"],
                        IdUsuario = reader.GetInt32("IdUsuario"),
                        FechaUltimaActualizacion = (DateTime)reader["FechaUltimaActualizacion"]
                    };


                    

            }

            return result;
        }

      

        

        public bool ExisteDuplicado(Producto producto)
        {
            return false;
        }
        public DataTable BuscarPorNombre(string frase)
        {
            frase = frase.ToLower();
            string query = @"SELECT Nombre
                    FROM producto 
                    WHERE Estado = 1 AND Nombre LIKE @frase 
                    ORDER BY Nombre ASC 
                    LIMIT 10";

            MySqlCommand command = new MySqlCommand(query);
            command.Parameters.AddWithValue("@frase", "%" + frase + "%");

            return ExecuteReturningDataTable(command);
        }
        public DataTable BuscarProducto(string nombre)
        {
            string query = @"
            SELECT 
                p.Id, 
                p.Nombre, 
                pp.Precio 
            FROM Producto p
            INNER JOIN PresentacionProducto pp ON p.Id = pp.IdProducto
            WHERE p.Nombre LIKE @nombre 
            AND p.Estado = 1 
            LIMIT 1";

            MySqlCommand command = new MySqlCommand(query);
            command.Parameters.AddWithValue("@nombre", "%" + nombre + "%");

            return ExecuteReturningDataTable(command);
        }

        public int RestaurarStock(int idProducto, int cantidad)
        {
            string query = @"UPDATE producto 
                             SET Stock = Stock + @cantidad, 
                                 FechaUltimaActualizacion = @fechaAhora 
                             WHERE Id = @idProducto;";
            
            MySqlCommand command = new MySqlCommand(query);
            command.Parameters.AddWithValue("@cantidad", cantidad);
            command.Parameters.AddWithValue("@idProducto", idProducto);
            command.Parameters.AddWithValue("@fechaAhora", DateTime.Now);
            return ExecuteNonQuery(command);
        }

        public int Insertar(Producto t)
        {
            string query = @"INSERT INTO producto ( Nombre,IdCategoria,IdMarca,Stock,FechaRegistro,IdUsuario)
                            VALUES (@nombre,@idCategoria,@idMarca,@stock,@fechaRegistro,@idUsuario);
                            SELECT LAST_INSERT_ID();";
            MySqlCommand command = new MySqlCommand(query);

            command.Parameters.AddWithValue("@nombre", t.Nombre);
            command.Parameters.AddWithValue("@idCategoria", t.IdCategoria);
            command.Parameters.AddWithValue("@idMarca", t.IdMarca);
            command.Parameters.AddWithValue("@stock", t.Stock);
            command.Parameters.AddWithValue("@fechaRegistro", t.FechaRegistro);
            command.Parameters.AddWithValue("@idUsuario", t.IdUsuario);
            return Convert.ToInt32(RepositorioBD.Instancia.ExecuteScalar(command));
        }

        public int Actualizar(Producto t)
        {
            string query = @"UPDATE bdlibreria.producto
                            SET IdCategoria = @idCategoria,
	                            Nombre = @nombre,
                                IdMarca = @idMarca,
                                Stock = @stock,
                                FechaUltimaActualizacion = @fechaAhora,
                                IdUsuario=@idUsuario
                                
                            WHERE Id = @id;";

            MySqlCommand command = new MySqlCommand(query);

            command.Parameters.AddWithValue("@idCategoria", t.IdCategoria);
            command.Parameters.AddWithValue("@nombre", t.Nombre);
            command.Parameters.AddWithValue("@idMarca", t.IdMarca);
            command.Parameters.AddWithValue("@stock", t.Stock);
            command.Parameters.AddWithValue("@fechaAhora", DateTime.Now);
            command.Parameters.AddWithValue("@id", t.Id);
            command.Parameters.AddWithValue("@idUsuario", t.IdUsuario);
            return ExecuteNonQuery(command);
        }

        public int Eliminar(Producto t)        {
            string query = @"UPDATE producto
                     SET Estado = 0, FechaUltimaActualizacion=@fechaAhora, IdUsuario=@idUsuario
                     WHERE Id = @Id";
            MySqlCommand cmd = new MySqlCommand(query);
            cmd.Parameters.AddWithValue("@fechaAhora", DateTime.Now);
            cmd.Parameters.AddWithValue("@idUsuario", t.IdUsuario);
            cmd.Parameters.AddWithValue("@Id", t.Id);

            return ExecuteNonQuery(cmd);
        }

        public List<Producto> ObtenerTodo()
        {
            string query = @"SELECT  Id, Nombre,IdCategoria,IdMarca,Stock,Estado,FechaRegistro,IdUsuario,FechaUltimaActualizacion
                            FROM producto
                            WHERE Estado=1
                            ORDER BY 3";
            MySqlCommand command = new MySqlCommand(query);
            var result = new List<Producto>();
            var  reader = ExecuteReader(command);
            while (reader.Read())
            {

                result.Add(
                    new Producto
                    {
                        Id = reader.GetInt32("Id"),
                        Nombre = reader["Nombre"].ToString(),
                        IdCategoria = reader.GetInt32("IdCategoria"),
                        IdMarca = reader.GetInt32("IdMarca"),
                        Stock = reader.GetInt32("Stock"),
                        Estado = (bool)reader["Estado"],
                        FechaRegistro = (DateTime)reader["FechaRegistro"],
                        IdUsuario = reader.GetInt32("IdUsuario"),
                        FechaUltimaActualizacion = (DateTime)reader["FechaUltimaActualizacion"]
                    }


                    );

            }
            return result;

        }
    }
}
