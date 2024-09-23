using CasualTasker.DTO;

namespace CasualTasker.Services.Fallbacks
{
    /// <summary>
    /// Provides access to fallback categories used in the application.
    /// </summary>
    public interface ICategoryFallbackService
    {
        /// <summary>
        /// Gets the name of the deleted category.
        /// </summary>
        string DeletedCategoryName { get; }

        /// <summary>
        /// Gets the deleted category as a <see cref="CategoryDTO"/> object.
        /// </summary>
        CategoryDTO DeletedCategory { get; }
        /// <summary>
        /// Gets the name of the common category used as a fallback.
        /// </summary>
        string CommonCategoryName { get; }
        /// <summary>
        /// Gets the common category as a <see cref="CategoryDTO"/> object.
        /// </summary>
        CategoryDTO CommonCategory { get; }
    }
}
