using CasualTasker.DTO;

namespace CasualTasker.Infrastructure.ObservableDbCollections
{
    /// <summary>
    /// Synchronizes tasks with categories, handling updates and deletions.
    /// </summary>
    public sealed class CategoryTaskSynchronizer
    {
        private readonly IObservableDbCollection<TaskDTO> _tasks;
        private readonly ObservableCategoryDbCollection _categories;

        /// <summary>
        /// Initializes a new instance of the CategoryTaskSynchronizer class.
        /// </summary>
        /// <param name="tasks">Collection of tasks to synchronize.</param>
        /// <param name="categories">Collection of categories to synchronize with.</param>
        public CategoryTaskSynchronizer(IObservableDbCollection<TaskDTO> tasks, ObservableCategoryDbCollection categories)
        {
            _tasks = tasks;
            _categories = categories;
        }

        /// <summary>
        /// Starts the synchronization process by subscribing to category updates and deletions.
        /// </summary>
        public void Synchronize()
        {
            _categories.OnCategoryUpdated += HandleCategoryUpdated;
            _categories.OnCategoryDeleted += HandleCategoryDeleted;
        }

        /// <summary>
        /// Handles the event when a category is deleted.
        /// Updates tasks that were associated with the deleted category.
        /// </summary>
        /// <param name="deletedCategory">The deleted category.</param>
        private void HandleCategoryDeleted(CategoryDTO deletedCategory)
        {
            var tasksToUpdate = _tasks.Entities
               .Where(task => task.Category.Id == deletedCategory.Id)
               .ToList();

            CategoryDTO defaultCategory = _categories.DefaultDeletedEntity;

            foreach (var task in tasksToUpdate)
            {
                task.Category = defaultCategory;
                _tasks.Update(task, false);
            }
        }

        /// <summary>
        /// Handles the event when a category is updated.
        /// Updates tasks that were associated with the updated category.
        /// </summary>
        /// <param name="updatedCategory">The updated category.</param>
        private void HandleCategoryUpdated(CategoryDTO updatedCategory)
        {
            var tasksToUpdate = _tasks.Entities.Where(task => task.Category.Id == updatedCategory.Id);

            foreach (var task in tasksToUpdate)
            {
                task.Category = updatedCategory;
                _tasks.Update(task, false);
            }
        }
    }
}
