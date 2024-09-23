using CasualTasker.Database.Context;
using CasualTasker.DTO;

namespace CasualTasker.Services.Fallbacks
{
    /// <summary>
    /// Implements the <see cref="ICategoryFallbackService"/> to provide access to fallback categories.
    /// </summary>
    public sealed class CategoryFallbackService : ICategoryFallbackService
    {
        private const string DELETED_CATEGORY_NAME = "Удалено";
        private const string COMMON_CATEGORY_NAME = "Общее";
        private readonly CasualTaskerDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryFallbackService"/> class.
        /// </summary>
        /// <param name="dbContext">The database context used to retrieve categories.</param>
        public CategoryFallbackService(CasualTaskerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets the name of the deleted category.
        /// </summary>
        public string DeletedCategoryName => DELETED_CATEGORY_NAME;

        /// <summary>
        /// Gets the deleted category as a <see cref="CategoryDTO"/> object.
        /// </summary>
        public CategoryDTO DeletedCategory =>
            _dbContext.Categories.FirstOrDefault(el => el.Name == DeletedCategoryName);

        /// <summary>
        /// Gets the name of the common category.
        /// </summary>
        public string CommonCategoryName => COMMON_CATEGORY_NAME;

        /// <summary>
        /// Gets the common category as a <see cref="CategoryDTO"/> object.
        /// </summary>
        public CategoryDTO CommonCategory =>
            _dbContext.Categories.FirstOrDefault(el => el.Name == CommonCategoryName);
    }
}
