using Microsoft.Data.SqlClient;

namespace PublicApiCaller.Data
{
    public class DbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection")
            );
        }
    }
}
