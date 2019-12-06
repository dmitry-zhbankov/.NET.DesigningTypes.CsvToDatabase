using System;

namespace Logger
{
    public interface ILogger : IDisposable
    {
        void Error(string message);

        void Error(Exception ex);

        void Warning(string message);

        void Info(string message);
    }
}