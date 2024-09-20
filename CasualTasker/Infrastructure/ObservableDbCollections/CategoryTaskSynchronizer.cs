using CasualTasker.DTO;

namespace CasualTasker.Infrastructure.ObservableDbCollections
{
    public class CategoryTaskSynchronizer
    {
        private readonly IObservableDbCollection<TaskDTO> _tasks;
        private readonly ObservableCategoryDbCollection _categories;
        public CategoryTaskSynchronizer(IObservableDbCollection<TaskDTO> tasks, ObservableCategoryDbCollection categories)
        {
            _tasks = tasks;
            _categories = categories;
        }

        public void Synchronize()
        {
            _categories.OnCategoryUpdated += HandleCategoryUpdated;
            _categories.OnCategoryDeleted += HandleCategoryDeleted;
        }

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
