using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasualTasker.Infrastructure.Middleware
{
    public interface IExceptionHandlingService
    {
        void HandleException(Exception exception);
    }

    public class ExceptionHandlingService : IExceptionHandlingService
    {
        private readonly ILogger<ExceptionHandlingService> _logger;
        public ExceptionHandlingService(ILogger<ExceptionHandlingService> logger)
        {
            _logger = logger;
        }

        public void HandleException(Exception exception)
        {
            _logger.LogError(exception, exception.Message);
        }
    }
}
