using CasualTasker.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CasualTasker.Infrastructure.DbInitializers
{
    public class CasualTaskerDbInitializer
    {
        private readonly ILogger _logger;
        private readonly CasualTaskerDbContext _dbContext;
        public CasualTaskerDbInitializer(ILogger<CasualTaskerDbInitializer> logger, CasualTaskerDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task InitializeAsync()
        {
            _logger.LogInformation($"Выполнение операции {nameof(InitializeAsync)} в классе {nameof(CasualTaskerDbInitializer)}");
            _dbContext.Database.Migrate();
        }
    }
}
