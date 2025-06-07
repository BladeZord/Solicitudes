using System.Data.SqlClient;

namespace es_usuario.utils
{
    public class DbConnectionManager : IDisposable
    {
        private readonly SqlConnection _sqlConnection;

        public DbConnectionManager(Provider Provider)
        {
            _sqlConnection = new SqlConnection(Provider.Url.SQLConnection);
        }

        public async Task<SqlConnection> ObtenerConexion()
        {
            if (_sqlConnection.State != System.Data.ConnectionState.Open)
            {
                await _sqlConnection.OpenAsync();
            }
            return _sqlConnection;
        }

        public async Task CerrarConexion()
        {
            if (_sqlConnection.State != System.Data.ConnectionState.Closed)
            {
                await _sqlConnection.CloseAsync();
            }
        }

        // Implementación de IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Liberar recursos gestionados (cerrar la conexión)
                _sqlConnection.Dispose();
            }
        }

        // Destructor
        ~DbConnectionManager()
        {
            Dispose(false);
        }
    }
}
