namespace CasualTasker.Infrastructure.Middleware
{
    /// <summary>
    /// Interface for handling exceptions.
    /// </summary>
    public interface IExceptionHandlingService
    {
        /// <summary>
        /// Handles the specified exception.
        /// </summary>
        /// <param name="exception">The exception to handle.</param>
        void HandleException(Exception exception);
    }
}
