using System;

namespace CityWebServer.Extensibility
{
    public interface ILogAppender
    {
        event EventHandler<LogAppenderEventArgs> LogMessage;
    }
}