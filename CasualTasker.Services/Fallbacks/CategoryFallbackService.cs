using CasualTasker.Database.Context;
using CasualTasker.DTO;

namespace CasualTasker.Services.Fallbacks
{
    public class CategoryFallbackService : ICategoryFallbackService
    {
        private const string DELETED_CATEGORY_NAME = "Удалено";
        private const string COMMON_CATEGORY_NAME = "Общее";
        private readonly CasualTaskerDbContext _dbContext;

        public CategoryFallbackService(CasualTaskerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string DeletedCategoryName => DELETED_CATEGORY_NAME;

        public CategoryDTO DeletedCategory =>
            _dbContext.Categories.FirstOrDefault(el => el.Name == DeletedCategoryName);

        public string CommonCategoryName => COMMON_CATEGORY_NAME;

        public CategoryDTO CommonCategory =>
            _dbContext.Categories.FirstOrDefault(el => el.Name == CommonCategoryName);
    }
}
