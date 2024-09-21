using CasualTasker.Database.Context;
using CasualTasker.DTO;
using CasualTasker.Services.Fallbacks;
using CasualTasker.Services.Stores;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CasualTasker.Services.Tests.Stores
{
    public class CategoryStoreTests
    {
        private readonly DbContextOptions<CasualTaskerDbContext> _options;
        private readonly Mock<ICategoryFallbackService> _mockCategoryFallbackService;
        private readonly Mock<ILogger<CategoryStore>> _mockLogger;

        public CategoryStoreTests()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            _options = new DbContextOptionsBuilder<CasualTaskerDbContext>()
                .UseSqlite(connection)
                .Options;

            _mockCategoryFallbackService = new Mock<ICategoryFallbackService>();
            _mockLogger = new Mock<ILogger<CategoryStore>>();
            _mockCategoryFallbackService
                .Setup(x => x.DeletedCategory)
                .Returns(new CategoryDTO
                {
                    Name = "Deleted"
                });
        }

        #region Add

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void AddCategory_ShouldThrowException(string name)
        {
            // Arrange
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            var categoryStore = new CategoryStore(context, _mockLogger.Object, _mockCategoryFallbackService.Object);
            var invalidCategory = new CategoryDTO { Name = name };
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => categoryStore.Add(invalidCategory));
            // Assert
            Assert.Equal("Параметр item=null или item.Name=null", exception.Message);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("Lol")]
        public void AddCategory_ShouldAddCategoryToDatabase(string name)
        {
            // Arrange
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            var categoryStore = new CategoryStore(context, _mockLogger.Object, _mockCategoryFallbackService.Object);
            var invalidCategory = new CategoryDTO { Name = name };
            // Act
            var addedCategory = categoryStore.Add(invalidCategory);
            context.SaveChanges();
            // Assert
            var storedCategory = context.Categories.FirstOrDefault(c => c.Name == name);
            Assert.NotNull(addedCategory);
            Assert.NotNull(storedCategory);
            Assert.Equal(addedCategory.Id, storedCategory.Id);
            Assert.Equal(addedCategory.Name, storedCategory.Name);
            Assert.Equal(addedCategory.Color, storedCategory.Color);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("Lol")]
        public void AddCategory_ShouldNotAddCategoryToDatabase(string name)
        {
            // Arrange
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            var categoryStore = new CategoryStore(context, _mockLogger.Object, _mockCategoryFallbackService.Object);
            var invalidCategory = new CategoryDTO { Name = name };
            // Act
            var addedCategory1 = categoryStore.Add(invalidCategory);
            context.SaveChanges();
            var addedCategory2 = categoryStore.Add(invalidCategory);
            context.SaveChanges();

            // Assert
            Assert.NotNull(addedCategory1);
            Assert.NotNull(addedCategory2);
            Assert.Equal(addedCategory1.Id, addedCategory2.Id);
            Assert.Equal(addedCategory1.Name, addedCategory2.Name);
            Assert.Equal(addedCategory1.Color, addedCategory2.Color);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task AddAsyncCategory_ShouldThrowException(string name)
        {
            // Arrange
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            var categoryStore = new CategoryStore(context, _mockLogger.Object, _mockCategoryFallbackService.Object);
            var invalidCategory = new CategoryDTO { Name = name };
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await categoryStore.AddAsync(invalidCategory));
            // Assert
            Assert.Equal("Параметр item=null или item.Name=null", exception.Message);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("Lol")]
        public async Task AddAsyncCategory_ShouldAddCategoryToDatabase(string name)
        {
            // Arrange
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            var categoryStore = new CategoryStore(context, _mockLogger.Object, _mockCategoryFallbackService.Object);
            var invalidCategory = new CategoryDTO { Name = name };
            // Act
            var addedCategory = await categoryStore.AddAsync(invalidCategory);
            context.SaveChanges();
            // Assert
            var storedCategory = context.Categories.FirstOrDefault(c => c.Name == name);
            Assert.NotNull(addedCategory);
            Assert.NotNull(storedCategory);
            Assert.Equal(addedCategory.Id, storedCategory.Id);
            Assert.Equal(addedCategory.Name, storedCategory.Name);
            Assert.Equal(addedCategory.Color, storedCategory.Color);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("Lol")]
        public async Task AddAsyncCategory_ShouldNotAddCategoryToDatabase(string name)
        {
            // Arrange
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            var categoryStore = new CategoryStore(context, _mockLogger.Object, _mockCategoryFallbackService.Object);
            var invalidCategory = new CategoryDTO { Name = name };
            // Act
            var addedCategory1 = await categoryStore.AddAsync(invalidCategory);
            context.SaveChanges();
            var addedCategory2 = await categoryStore.AddAsync(invalidCategory);
            context.SaveChanges();

            // Assert
            Assert.NotNull(addedCategory1);
            Assert.NotNull(addedCategory2);
            Assert.Equal(addedCategory1.Id, addedCategory2.Id);
            Assert.Equal(addedCategory1.Name, addedCategory2.Name);
            Assert.Equal(addedCategory1.Color, addedCategory2.Color);
        }


        #endregion

        #region Delete

        [Theory]
        [InlineData("TestName")]
        [InlineData("DeleteItem")]
        public void DeleteCategory_ShouldRemoveCategoryFromDatabase(string name)
        {
            // Arrange
            bool exeptedResult = true;
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            var categoryStore = new CategoryStore(context, _mockLogger.Object, _mockCategoryFallbackService.Object);
            var category = new CategoryDTO { Name = name };
            categoryStore.Add(category);
            categoryStore.Add(_mockCategoryFallbackService.Object.DeletedCategory);
            context.SaveChanges();

            // Act
            var result = categoryStore.Delete(category.Id);
            context.SaveChanges();

            // Assert
            var deletedCategory = context.Categories.FirstOrDefault(c => c.Name == name);
            Assert.Null(deletedCategory);
            Assert.Equal(result, exeptedResult);
        }

        [Theory]
        [InlineData("TestName")]
        [InlineData("DeleteItem")]
        public void DeleteCategory_ShouldRemoveCategoryAndReSetTasksCategoryFromDatabase(string name)
        {
            // Arrange
            bool exeptedResult = true;
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            var categoryStore = new CategoryStore(context, _mockLogger.Object, _mockCategoryFallbackService.Object);
            var category = new CategoryDTO { Name = name };
            var addedCategory = categoryStore.Add(category);
            categoryStore.Add(_mockCategoryFallbackService.Object.DeletedCategory);
            context.SaveChanges();
            var tasks = Enumerable.Range(0, 5).Select(i => new TaskDTO
            {
                Name = $"Task-{i}",
                Category = context.Categories.FirstOrDefault(el => el.Id == addedCategory.Id),
                Status = CasualTaskStatus.Completed,
                Description = "",
                DueDate = DateTime.Now,
            }).ToList();

            for (int i = 0; i < tasks.Count; i++)
            {
                if (tasks[i].Category != null)
                {
                    context.Entry(tasks[i].Category).State = EntityState.Unchanged;
                }

                // Добавляем задачу
                var addedTaskEntire = context.Tasks.Add(tasks[i]);
                tasks[i].Id = addedTaskEntire.Entity.Id;
                // Сохраняем изменения
                context.SaveChanges();

                context.ChangeTracker.Clear();
            }

            // Act
            var result = categoryStore.Delete(category.Id);
            context.SaveChanges();

            // Assert
            var deletedCategory = context.Categories.FirstOrDefault(c => c.Name == name);
            var tasksIds = tasks.Select(ts => ts.Id).ToList();
            var newTasks = context.Tasks.Include(el => el.Category).Where(el => tasksIds.Contains(el.Id));
            Assert.Null(deletedCategory);
            Assert.Equal(result, exeptedResult);

            foreach (var newTask in newTasks)
            {
                Assert.NotNull(newTask.Category);
                Assert.Equal(newTask.Category.Id, _mockCategoryFallbackService.Object.DeletedCategory.Id);
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        public void DeleteCategory_ShouldNotRemoveCategoryFromDatabase(int id)
        {
            // Arrange
            bool exeptedResult = false;
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            var categoryStore = new CategoryStore(context, _mockLogger.Object, _mockCategoryFallbackService.Object);
            // Act
            var result = categoryStore.Delete(id);
            context.SaveChanges();

            // Assert
            var deletedCategory = context.Categories.FirstOrDefault(c => c.Id == id);
            Assert.Null(deletedCategory);
            Assert.Equal(result, exeptedResult);
        }

        [Theory]
        [InlineData("TestName")]
        [InlineData("DeleteItem")]
        public async Task DeleteAsyncCategory_ShouldRemoveCategoryFromDatabase(string name)
        {
            // Arrange
            bool exeptedResult = true;
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            var categoryStore = new CategoryStore(context, _mockLogger.Object, _mockCategoryFallbackService.Object);
            var category = new CategoryDTO { Name = name };
            categoryStore.Add(_mockCategoryFallbackService.Object.DeletedCategory);
            categoryStore.Add(category);
            context.SaveChanges();

            // Act
            var result = await categoryStore.DeleteAsync(category.Id);
            context.SaveChanges();

            // Assert
            var deletedCategory = context.Categories.FirstOrDefault(c => c.Name == name);
            Assert.Null(deletedCategory);
            Assert.Equal(result, exeptedResult);
        }

        [Theory]
        [InlineData("TestName")]
        [InlineData("DeleteItem")]
        public async Task DeleteAsyncCategory_ShouldRemoveCategoryAndReSetTasksCategoryFromDatabase(string name)
        {
            // Arrange
            bool exeptedResult = true;
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            var categoryStore = new CategoryStore(context, _mockLogger.Object, _mockCategoryFallbackService.Object);
            var category = new CategoryDTO { Name = name };
            var addedCategory = categoryStore.Add(category);
            categoryStore.Add(_mockCategoryFallbackService.Object.DeletedCategory);
            await context.SaveChangesAsync();

            var tasks = Enumerable.Range(0, 5).Select(i => new TaskDTO
            {
                Name = $"Task-{i}",
                Category = context.Categories.FirstOrDefault(el => el.Id == addedCategory.Id),
                Status = CasualTaskStatus.Completed,
                Description = "",
                DueDate = DateTime.Now,
            }).ToList();

            for (int i = 0; i < tasks.Count; i++)
            {
                if (tasks[i].Category != null)
                {
                    context.Entry(tasks[i].Category).State = EntityState.Unchanged;
                }

                // Добавляем задачу
                var addedTaskEntire = context.Tasks.Add(tasks[i]);
                tasks[i].Id = addedTaskEntire.Entity.Id;
                // Сохраняем изменения
                await context.SaveChangesAsync();

                context.ChangeTracker.Clear();
            }

            // Act
            var result = await categoryStore.DeleteAsync(category.Id);
            context.SaveChanges();

            // Assert
            var deletedCategory = context.Categories.FirstOrDefault(c => c.Name == name);
            var tasksIds = tasks.Select(ts => ts.Id).ToList();
            var newTasks = context.Tasks.Include(el => el.Category).Where(el => tasksIds.Contains(el.Id));
            Assert.Null(deletedCategory);
            Assert.Equal(result, exeptedResult);

            foreach (var newTask in newTasks)
            {
                Assert.NotNull(newTask.Category);
                Assert.Equal(newTask.Category.Id, _mockCategoryFallbackService.Object.DeletedCategory.Id);
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        public async Task DeleteAsyncCategory_ShouldNotRemoveCategoryFromDatabase(int id)
        {
            // Arrange
            bool exeptedResult = false;
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            var categoryStore = new CategoryStore(context, _mockLogger.Object, _mockCategoryFallbackService.Object);
            // Act
            var result = await categoryStore.DeleteAsync(id);
            context.SaveChanges();

            // Assert
            var deletedCategory = context.Categories.FirstOrDefault(c => c.Id == id);
            Assert.Null(deletedCategory);
            Assert.Equal(result, exeptedResult);
        }

        #endregion

        #region Update

        [Theory]
        [InlineData("StartName", "NewName")]
        [InlineData("StartName", "TestName")]
        public void UpdateCategory_ShouldUpdateCategoryFromDatabase(string startName, string newName)
        {
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            var categoryStore = new CategoryStore(context, _mockLogger.Object, _mockCategoryFallbackService.Object);
            var category = new CategoryDTO { Name = startName };
            var addedCategory = categoryStore.Add(category);
            context.SaveChanges();

            // Act
            addedCategory.Name = newName;
            var result = categoryStore.Update(addedCategory);
            context.SaveChanges();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result.Name, addedCategory.Name);
            Assert.Equal(result.Name, newName);
            Assert.NotEqual(result.Name, startName);
        }

        [Theory]
        [InlineData("StartName", "")]
        [InlineData("StartName", null)]
        public void UpdateCategory_ShouldThrowException(string startName, string newName)
        {
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            var categoryStore = new CategoryStore(context, _mockLogger.Object, _mockCategoryFallbackService.Object);
            var category = new CategoryDTO { Name = startName };
            var addedCategory = categoryStore.Add(category);
            context.SaveChanges();

            // Act
            addedCategory.Name = newName;
            var exception = Assert.Throws<ArgumentNullException>(() => categoryStore.Update(addedCategory));
            // Assert
            Assert.Equal("Параметр item=null или item.Name=null", exception.Message);
        }

        [Theory]
        [InlineData("StartName")]
        [InlineData("N")]
        public void UpdateCategory_ShouldReturnNull(string newName)
        {
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            var categoryStore = new CategoryStore(context, _mockLogger.Object, _mockCategoryFallbackService.Object);
            var category = new CategoryDTO { Name = newName };

            // Act
            var updatedCategory = categoryStore.Update(category);
            // Assert
            Assert.Null(updatedCategory);
        }

        [Theory]
        [InlineData("StartName", "NewName")]
        [InlineData("StartName", "TestName")]
        public async Task UpdateAsyncCategory_ShouldUpdateCategoryFromDatabase(string startName, string newName)
        {
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            var categoryStore = new CategoryStore(context, _mockLogger.Object, _mockCategoryFallbackService.Object);
            var category = new CategoryDTO { Name = startName };
            var addedCategory = categoryStore.Add(category);
            context.SaveChanges();

            // Act
            addedCategory.Name = newName;
            var result = await categoryStore.UpdateAsync(addedCategory);
            context.SaveChanges();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result.Name, addedCategory.Name);
            Assert.Equal(result.Name, newName);
            Assert.NotEqual(result.Name, startName);
        }

        [Theory]
        [InlineData("StartName", "")]
        [InlineData("StartName", null)]
        public async Task UpdateAsyncCategory_ShouldThrowException(string startName, string newName)
        {
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            var categoryStore = new CategoryStore(context, _mockLogger.Object, _mockCategoryFallbackService.Object);
            var category = new CategoryDTO { Name = startName };
            var addedCategory = categoryStore.Add(category);
            context.SaveChanges();

            // Act
            addedCategory.Name = newName;
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await categoryStore.UpdateAsync(addedCategory));
            // Assert
            Assert.Equal("Параметр item=null или item.Name=null", exception.Message);
        }

        [Theory]
        [InlineData("StartName")]
        [InlineData("N")]
        public async Task UpdateAsyncCategory_ShouldReturnNull(string newName)
        {
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            var categoryStore = new CategoryStore(context, _mockLogger.Object, _mockCategoryFallbackService.Object);
            var category = new CategoryDTO { Name = newName };

            // Act
            var updatedCategory = await categoryStore.UpdateAsync(category);
            // Assert
            Assert.Null(updatedCategory);
        }

        #endregion
    }
}
