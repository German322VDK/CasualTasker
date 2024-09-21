using CasualTasker.DTO;

namespace CasualTasker.Services.Fallbacks
{
    public interface ICategoryFallbackService
    {
        string DeletedCategoryName { get; }
        CategoryDTO DeletedCategory { get; }
        string CommonCategoryName { get; }
        CategoryDTO CommonCategory { get; }
    }
}
