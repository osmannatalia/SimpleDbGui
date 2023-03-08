using MySql.Data.MySqlClient;
using System.Data;

namespace SimpleDbGui.DataAcessLayer
{
    internal class ConnectionProvider
    {
        public static string ConnectionString { get; set; } = string.Empty;
        public static IDbConnection ConnectionGet() {
            try
            { 
                var connection = new MySqlConnection(ConnectionString);
                connection.Open();
                return connection; 
            }
            catch (Exception ex) {
                throw new IOException($"Could not connect to Database!\n\n{ex.Message}", ex);
            }   
        }
    }
}
