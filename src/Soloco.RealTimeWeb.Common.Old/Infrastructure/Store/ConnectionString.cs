using System;
using System.Configuration;
using System.Data.Common;
using Npgsql;

namespace Soloco.RealTimeWeb.Common.Infrastructure.Store
{
    public class ConnectionString
    {
        public string Server { get; }
        public string Port { get; }
        public string Database { get; }
        public string Password { get; }
        public string UserId { get; }

        private ConnectionString(string server, string port, string database, string userId, string password)
        {
            Port = port;
            Database = database;
            Password = password;
            UserId = userId;
            Server = server;
        }

        public static string GetString(string name = "documentStore")
        {
            var connectionString = ConfigurationManager.ConnectionStrings[name];
            if (string.IsNullOrWhiteSpace(connectionString?.ConnectionString))
            {
                throw new InvalidOperationException($"ConnectionString '{name}' not found in app.config");
            }
            return connectionString.ConnectionString;
        }
        
        public static ConnectionString Parse(string name = "documentStore")
        {
            using (var connection = new NpgsqlConnection())
            {
                var factory = DbProviderFactories.GetFactory(connection);
                var builder = factory.CreateConnectionStringBuilder();
                builder.ConnectionString = GetString(name);

                return new ConnectionString(
                    GetPart(builder, "Server"),
                    GetPart(builder, "Port"),
                    GetPart(builder, "database"),
                    GetPart(builder, "User Id"),
                    GetPart(builder, "password")
                    );
            }
        }

        private static string GetPart(DbConnectionStringBuilder builder, string name)
        {
            return builder[name]?.ToString();
        }
    }
}