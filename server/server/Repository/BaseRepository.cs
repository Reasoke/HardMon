using server.Classes;
using System.Data;
using System.Data.SqlClient;

namespace server.Repository
{

    public abstract class BaseRepository {

        private readonly string connectionString;

        protected BaseRepository(ISettingsProvider settingsProvider) {
            connectionString = settingsProvider.GetValue<string>("MainDb");
        }

        protected IDbConnection GetConnection() {
            return new SqlConnection(connectionString);
        }
    }
}
