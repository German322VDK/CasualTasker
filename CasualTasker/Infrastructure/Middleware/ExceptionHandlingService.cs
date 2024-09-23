using Microsoft.Extensions.Logging;

namespace CasualTasker.Infrastructure.Middleware
{
    /// <summary>
    /// Implementation of the exception handling service that logs exceptions.
    /// </summary>
    public class ExceptionHandlingService : IExceptionHandlingService
    {
        private readonly ILogger<ExceptionHandlingService> _logger;

        /// <summary>
        /// Initializes a new instance of the ExceptionHandlingService class.
        /// </summary>
        /// <param name="logger">The logger to use for logging exceptions.</param>
        public ExceptionHandlingService(ILogger<ExceptionHandlingService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Handles the specified exception by logging it.
        /// </summary>
        /// <param name="exception">The exception to handle.</param>
        public void HandleException(Exception exception)
        {
            _logger.LogError(exception, exception.Message);
        }
    }
}
