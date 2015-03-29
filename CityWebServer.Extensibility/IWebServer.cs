namespace CityWebServer.Extensibility
{
    public interface IWebServer
    {
        /// <summary>
        /// Gets an array containing all currently registered request handlers.
        /// </summary>
        IRequestHandler[] RequestHandlers { get; }
    }
}
