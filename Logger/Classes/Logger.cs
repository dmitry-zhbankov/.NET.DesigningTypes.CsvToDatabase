using System;
using System.Collections.Generic;

namespace Logger.Classes
{
    public class Logger : ILogger
    {
        private readonly ILogger logger;
        bool disposed = false;

        public Logger()
        {
            logger = new ConsoleLogger();
        }

        public Logger(ILogger logger)
        {
            this.logger = logger;
        }

        public void Error(string message)
        {
            logger.Error(message);
        }

        public void Error(Exception ex)
        {
            logger.Error(ex);
        }

        public void Info(string message)
        {
            logger.Info(message);
        }

        public void Warning(string message)
        {
            logger.Warning(message);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            Console.WriteLine("BaseLogger disposer");
            if (disposed)
                return;
            if (disposing)
            {
            }
            logger.Dispose();
            disposed = true;
        }

        ~Logger()
        {
            Console.WriteLine("BaseLogger destructor");
            Dispose(false);
        }
    }
}
