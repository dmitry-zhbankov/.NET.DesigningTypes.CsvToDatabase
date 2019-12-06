using Microsoft.Data.Sqlite;
using System;

namespace Logger
{
    public class DbLogger : ILogger, IDisposable
    {
        private static SqliteConnection connection;
        private bool disposed = false;
        private static DbLogger instance;
        private ILogger logger;

        private DbLogger()
        {
            connection = new SqliteConnection(@"Data Source=log.db");
            var sqlExpression =
                @"CREATE TABLE IF NOT EXISTS Logs(
                Id INTEGER PRIMARY KEY,
                Time Text NOT NULL,
                Content TEXT NOT NULL
                );";
            connection?.Open();
            var command = new SqliteCommand(sqlExpression, connection);
            command.ExecuteNonQuery();
            command.Dispose();
        }

        private DbLogger(ILogger logger) : this()
        {
            this.logger = logger;
        }

        public static DbLogger GetInstance(ILogger logger = null)
        {
            if (instance != null)
            {
                if (logger == null)
                {
                    instance.logger = null;
                }
                return instance;
            }
            instance = new DbLogger(logger);
            return instance;
        }

        public void Error(string message)
        {
            var sqlExpression = $"INSERT INTO Logs(Content, Time) VALUES('Error: {message}', datetime('now','localtime'))";
            var command = new SqliteCommand(sqlExpression, connection);
            command.ExecuteNonQuery();
            command.Dispose();
            logger?.Error(message);
        }

        public void Error(Exception ex)
        {
            var sqlExpression = $"INSERT INTO Logs(Content, Time) VALUES('Error exception: {ex?.Message}', datetime('now','localtime'))";
            var command = new SqliteCommand(sqlExpression, connection);
            command.ExecuteNonQuery();
            command.Dispose();
            logger?.Error(ex);
        }

        public void Info(string message)
        {
            var sqlExpression = $"INSERT INTO Logs(Content, Time) VALUES('Info: {message}', datetime('now','localtime'))";
            var command = new SqliteCommand(sqlExpression, connection);
            command.ExecuteNonQuery();
            command.Dispose();
            logger?.Info(message);
        }

        public void Warning(string message)
        {
            var sqlExpression = $"INSERT INTO Logs(Content, Time) VALUES('Warning: {message}', datetime('now','localtime'))";
            var command = new SqliteCommand(sqlExpression, connection);
            command.ExecuteNonQuery();
            command.Dispose();
            logger?.Warning(message);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            Console.WriteLine("DbLogger disposer");
            if (disposed)
                return;
            if (disposing)
            {
            }
            instance = null;
            connection?.Close();
            connection?.Dispose();
            disposed = true;
        }

        ~DbLogger()
        {
            Console.WriteLine("DbLogger destructor");
            Dispose(false);
        }
    }
}
