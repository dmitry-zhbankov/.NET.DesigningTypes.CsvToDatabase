using System;

namespace Logger.Classes
{
    public class ConsoleLogger : ILogger
    {
        private readonly ILogger logger;
        private bool disposed = false;

        public ConsoleLogger()
        {
        }

        public ConsoleLogger(ILogger logger)
        {
            this.logger = logger;
        }

        public void Error(string message)
        {
            Console.WriteLine($"Log {DateTime.Now:s} Error: {message}");
            logger?.Error(message);
        }

        public void Error(Exception ex)
        {
            Console.WriteLine($"Log {DateTime.Now:s} Error exception: {ex?.Message}");
            logger?.Error(ex);
        }

        public void Info(string message)
        {
            Console.WriteLine($"Log {DateTime.Now:s} Info: {message}");
            logger?.Info(message);
        }

        public void Warning(string message)
        {
            Console.WriteLine($"Log {DateTime.Now:s} Warning: {message}");
            logger?.Warning(message);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Console.WriteLine("DbLogger disposer");
            if (disposed)
                return;
            if (disposing)
            {
            }
            logger?.Dispose();
        }

        ~ConsoleLogger()
        {
            Dispose(false);
        }
    }
}
