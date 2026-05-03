using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using System.Threading;

namespace Servicio_Clientes.Infraestructura.Persistencia
{
    public class ConexionBD
    {
        private static string? _connectionString;
        private static readonly Lazy<ConexionBD> _instancia = new Lazy<ConexionBD>(() => new ConexionBD());

        // Soporte para transacciones concurrentes por hilo/tarea
        private readonly AsyncLocal<MySqlTransaction?> _activeTransaction = new();
        private readonly AsyncLocal<MySqlConnection?> _activeConnection = new();

        public static ConexionBD Instancia
        {
            get
            {
                return _instancia.Value;
            }
        }

        private string CatchStringConnection()
        {
            return _connectionString
                ?? throw new InvalidOperationException("La cadena de conexión no ha sido configurada. Por favor, configure la cadena de conexión antes de usar el repositorio.");
        }

        public void Initiate(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString", "La cadena de conexión no puede ser nula o vacía.");
            _connectionString = connectionString;
        }

        #region Manejo de Transacciones
        public void BeginTransaction()
        {
            if (_activeTransaction.Value != null) return;

            var connection = new MySqlConnection(CatchStringConnection());
            connection.Open();
            var transaction = connection.BeginTransaction();

            _activeConnection.Value = connection;
            _activeTransaction.Value = transaction;
        }

        public void Commit()
        {
            try
            {
                _activeTransaction.Value?.Commit();
            }
            finally
            {
                CloseAndClearTransaction();
            }
        }

        public void Rollback()
        {
            try
            {
                _activeTransaction.Value?.Rollback();
            }
            finally
            {
                CloseAndClearTransaction();
            }
        }

        private void CloseAndClearTransaction()
        {
            if (_activeConnection.Value != null)
            {
                if (_activeConnection.Value.State == ConnectionState.Open)
                    _activeConnection.Value.Close();
                _activeConnection.Value.Dispose();
            }
            _activeConnection.Value = null;
            _activeTransaction.Value = null;
        }
        #endregion

        public int ExecuteNonQuery(MySqlCommand comando)
        {
            if (_activeTransaction.Value != null)
            {
                comando.Connection = _activeConnection.Value;
                comando.Transaction = _activeTransaction.Value;
                return comando.ExecuteNonQuery();
            }

            using (MySqlConnection con = new MySqlConnection(CatchStringConnection()))
            {
                con.Open();
                comando.Connection = con;
                return comando.ExecuteNonQuery();
            }
        }

        public MySqlDataReader ExecuteReader(MySqlCommand comando)
        {
            if (_activeTransaction.Value != null)
            {
                comando.Connection = _activeConnection.Value;
                comando.Transaction = _activeTransaction.Value;
                return comando.ExecuteReader();
            }

            MySqlConnection con = new MySqlConnection(CatchStringConnection());
            con.Open();
            comando.Connection = con;
            return comando.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public MySqlDataAdapter ExecuteDataAdapter(MySqlCommand comando)
        {
            if (_activeTransaction.Value != null)
            {
                comando.Connection = _activeConnection.Value;
                comando.Transaction = _activeTransaction.Value;
                return new MySqlDataAdapter(comando);
            }

            MySqlConnection con = new MySqlConnection(CatchStringConnection());
            comando.Connection = con;
            return new MySqlDataAdapter(comando);
        }

        public List<T> ExecuteList<T>(MySqlCommand comando, Func<MySqlDataReader, T> map)
        {
            List<T> lista = new List<T>();

            if (_activeTransaction.Value != null)
            {
                comando.Connection = _activeConnection.Value;
                comando.Transaction = _activeTransaction.Value;
                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(map(reader));
                    }
                }
                return lista;
            }

            using (MySqlConnection con = new MySqlConnection(CatchStringConnection()))
            {
                con.Open();
                comando.Connection = con;
                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(map(reader));
                    }
                }
            }
            return lista;
        }

        public object? ExecuteScalar(MySqlCommand comando)
        {
            if (_activeTransaction.Value != null)
            {
                comando.Connection = _activeConnection.Value;
                comando.Transaction = _activeTransaction.Value;
                return comando.ExecuteScalar();
            }

            using (MySqlConnection con = new MySqlConnection(CatchStringConnection()))
            {
                con.Open();
                comando.Connection = con;
                return comando.ExecuteScalar();
            }
        }
    }
}
