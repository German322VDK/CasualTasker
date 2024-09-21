using CasualTasker.Database.Context;
using CasualTasker.DTO;
using CasualTasker.Services.Fallbacks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CasualTasker.Services.Stores
{
    public class TaskStore : DbStore<TaskDTO>
    {
        private readonly ILogger<TaskStore> _logger;
        private readonly CasualTaskerDbContext _dbContext;
        private readonly ICategoryFallbackService _categoryFallbackService;

        public TaskStore(
            CasualTaskerDbContext dbContext,
            ICategoryFallbackService categoryFallbackService,
            ILogger<TaskStore> logger) : base(dbContext, logger)
        {
            _dbContext = dbContext;
            _categoryFallbackService = categoryFallbackService;
            _logger = logger;
        }

        public override TaskDTO Add(TaskDTO item)
        {
            _logger.LogInformation($"Начало работы метода {nameof(Add)} класса {nameof(TaskStore)}");

            ValidationEntityWithException(item, nameof(Add), nameof(TaskStore));

            item = SetCategory(item, nameof(Add), nameof(TaskStore));

            return base.Add(item);
        }

        public override async Task<TaskDTO> AddAsync(TaskDTO item)
        {
            _logger.LogInformation($"Начало работы метода {nameof(AddAsync)} класса {nameof(TaskStore)}");

            ValidationEntityWithException(item, nameof(Add), nameof(TaskStore));

            item = SetCategory(item, nameof(Add), nameof(TaskStore));

            return await base.AddAsync(item);
        }

        public override IQueryable<TaskDTO> GetAll() =>
            _dbContext.Tasks.Include(el => el.Category);

        public override TaskDTO GetById(int id) =>
            GetAll().FirstOrDefault(el => el.Id == id);

        private TaskDTO SetCategory(TaskDTO item, string methodName, string className)
        {
            if (item.Category == null)
            {
                _logger.LogInformation($"Категория элемента равна null поэтому её нужно инициализировать категорией по умолчанию" +
                    $" в методе {methodName} класса {className}");
                item.Category = _categoryFallbackService.CommonCategory;
            }
            else
            {
                item.Category = _dbContext.Categories.FirstOrDefault(el => el.Id == item.Category.Id);
            }

            return item;
        }
    }
}
