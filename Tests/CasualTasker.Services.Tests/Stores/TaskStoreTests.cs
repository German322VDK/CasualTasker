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
    public class TaskStoreTests
    {
        private readonly DbContextOptions<CasualTaskerDbContext> _options;
        private readonly Mock<ILogger<TaskStore>> _mockLogger;
        private readonly Mock<ICategoryFallbackService> _mockCategoryFallbackService;
        public TaskStoreTests()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            _options = new DbContextOptionsBuilder<CasualTaskerDbContext>()
                .UseSqlite(connection)
                .Options;

            _mockLogger = new Mock<ILogger<TaskStore>>();

            _mockCategoryFallbackService = new Mock<ICategoryFallbackService>();
            _mockCategoryFallbackService
                .Setup(x => x.DeletedCategory)
                .Returns(new CategoryDTO
                {
                    Name = "Deleted"
                });

            _mockCategoryFallbackService
                .Setup(x => x.CommonCategory)
                .Returns(new CategoryDTO
                {
                    Name = "Common"
                });
        }


        #region Add

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void AddTask_ShouldThrowException(string name)
        {
            // Arrange
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            var taskStore = new TaskStore(context, _mockCategoryFallbackService.Object, _mockLogger.Object);
            var invalidTask = new TaskDTO { Name = name };
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => taskStore.Add(invalidTask));
            // Assert
            Assert.Equal("Параметр item=null или item.Name=null", exception.Message);
        }

        [Theory]
        [InlineData("Task")]
        [InlineData("...")]
        public void AddTask_ShouldSetNullCategory(string name)
        {
            // Arrange
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            context.Categories.Add(_mockCategoryFallbackService.Object.CommonCategory);
            var taskStore = new TaskStore(context, _mockCategoryFallbackService.Object, _mockLogger.Object);
            var task = new TaskDTO
            {
                Name = name,
                Status = CasualTaskStatus.Postponed,
                Category = null,
                Description = "",
                DueDate = DateTime.Now,
            };

            // Act
            var addedTask = taskStore.Add(task);

            // Assert
            Assert.NotNull(addedTask);
            Assert.NotNull(addedTask.Category);
            Assert.Equal(name, addedTask.Name);
            Assert.Equal(_mockCategoryFallbackService.Object.CommonCategory.Name, addedTask.Category.Name);
        }

        [Theory]
        [InlineData("Task", "Category")]
        [InlineData("TaskName", "CategoryName")]
        public void AddTask_ShouldAddTaskToDatabase(string taskName, string categoryName)
        {
            // Arrange
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            var category = new CategoryDTO
            {
                Name = categoryName,
            };
            var addedCategory = context.Categories.Add(category);
            context.Categories.Add(_mockCategoryFallbackService.Object.CommonCategory);
            context.SaveChanges();
            var taskStore = new TaskStore(context, _mockCategoryFallbackService.Object, _mockLogger.Object);
            var task = new TaskDTO
            {
                Name = taskName,
                Status = CasualTaskStatus.Postponed,
                Category = addedCategory.Entity,
                Description = "",
                DueDate = DateTime.Now,
            };

            // Act
            var addedTask = taskStore.Add(task);

            // Assert
            Assert.NotNull(addedTask);
            Assert.NotNull(addedTask.Category);
            Assert.Equal(taskName, addedTask.Name);
            Assert.Equal(addedCategory.Entity.Name, addedTask.Category.Name);
            Assert.NotEqual(_mockCategoryFallbackService.Object.CommonCategory.Name, addedTask.Category.Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task AddAsyncTask_ShouldThrowException(string name)
        {
            // Arrange
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            var taskStore = new TaskStore(context, _mockCategoryFallbackService.Object, _mockLogger.Object);
            var invalidTask = new TaskDTO { Name = name };
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await taskStore.AddAsync(invalidTask));
            // Assert
            Assert.Equal("Параметр item=null или item.Name=null", exception.Message);
        }

        [Theory]
        [InlineData("Task")]
        [InlineData("...")]
        public async Task AddAsyncTask_ShouldSetNullCategory(string name)
        {
            // Arrange
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            context.Categories.Add(_mockCategoryFallbackService.Object.CommonCategory);
            var taskStore = new TaskStore(context, _mockCategoryFallbackService.Object, _mockLogger.Object);
            var task = new TaskDTO
            {
                Name = name,
                Status = CasualTaskStatus.Postponed,
                Category = null,
                Description = "",
                DueDate = DateTime.Now,
            };

            // Act
            var addedTask = await taskStore.AddAsync(task);

            // Assert
            Assert.NotNull(addedTask);
            Assert.NotNull(addedTask.Category);
            Assert.Equal(name, addedTask.Name);
            Assert.Equal(_mockCategoryFallbackService.Object.CommonCategory.Name, addedTask.Category.Name);
        }

        [Theory]
        [InlineData("Task", "Category")]
        [InlineData("TaskName", "CategoryName")]
        public async Task AddAsyncTask_ShouldAddTaskToDatabase(string taskName, string categoryName)
        {
            // Arrange
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            var category = new CategoryDTO
            {
                Name = categoryName,
            };
            var addedCategory = context.Categories.Add(category);
            context.Categories.Add(_mockCategoryFallbackService.Object.CommonCategory);
            context.SaveChanges();
            var taskStore = new TaskStore(context, _mockCategoryFallbackService.Object, _mockLogger.Object);
            var task = new TaskDTO
            {
                Name = taskName,
                Status = CasualTaskStatus.Postponed,
                Category = addedCategory.Entity,
                Description = "",
                DueDate = DateTime.Now,
            };

            // Act
            var addedTask = await taskStore.AddAsync(task);

            // Assert
            Assert.NotNull(addedTask);
            Assert.NotNull(addedTask.Category);
            Assert.Equal(taskName, addedTask.Name);
            Assert.Equal(addedCategory.Entity.Name, addedTask.Category.Name);
            Assert.NotEqual(_mockCategoryFallbackService.Object.CommonCategory.Name, addedTask.Category.Name);
        }

        #endregion

        #region Delete

        [Theory]
        [InlineData("TestName")]
        [InlineData("DeleteItem")]
        public void DeleteTask_ShouldRemoveTaskFromDatabase(string name)
        {
            // Arrange
            bool exeptedResult = true;
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            context.Categories.Add(_mockCategoryFallbackService.Object.CommonCategory);
            var taskStore = new TaskStore(context, _mockCategoryFallbackService.Object, _mockLogger.Object);
            var task = new TaskDTO
            {
                Name = name,
                Status = CasualTaskStatus.Postponed,
                Category = null,
                Description = "",
                DueDate = DateTime.Now,
            };
            var addedTask = taskStore.Add(task);

            // Act
            var result = taskStore.Delete(addedTask.Id);
            context.SaveChanges();

            // Assert
            var deletedTask = context.Tasks.FirstOrDefault(c => c.Name == name);
            Assert.Null(deletedTask);
            Assert.Equal(result, exeptedResult);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void DeleteTask_ShouldNotRemoveTaskFromDatabase(int id)
        {
            // Arrange
            bool exeptedResult = false;
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            var taskStore = new TaskStore(context, _mockCategoryFallbackService.Object, _mockLogger.Object);

            // Act
            var result = taskStore.Delete(id);
            context.SaveChanges();

            // Assert
            Assert.Equal(result, exeptedResult);
        }

        [Theory]
        [InlineData("TestName")]
        [InlineData("DeleteItem")]
        public async Task DeleteAsyncTask_ShouldRemoveTaskFromDatabase(string name)
        {
            // Arrange
            bool exeptedResult = true;
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            context.Categories.Add(_mockCategoryFallbackService.Object.CommonCategory);
            var taskStore = new TaskStore(context, _mockCategoryFallbackService.Object, _mockLogger.Object);
            var task = new TaskDTO
            {
                Name = name,
                Status = CasualTaskStatus.Postponed,
                Category = null,
                Description = "",
                DueDate = DateTime.Now,
            };
            var addedTask = taskStore.Add(task);

            // Act
            var result = await taskStore.DeleteAsync(addedTask.Id);
            await context.SaveChangesAsync();

            // Assert
            var deletedTask = context.Tasks.FirstOrDefault(c => c.Name == name);
            Assert.Null(deletedTask);
            Assert.Equal(result, exeptedResult);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task DeleteAsyncTask_ShouldNotRemoveTaskFromDatabase(int id)
        {
            // Arrange
            bool exeptedResult = false;
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();
            var taskStore = new TaskStore(context, _mockCategoryFallbackService.Object, _mockLogger.Object);

            // Act
            var result = await taskStore.DeleteAsync(id);
            await context.SaveChangesAsync();

            // Assert
            Assert.Equal(result, exeptedResult);
        }

        #endregion

        #region Update

        [Theory]
        [InlineData("StartName", "NewName")]
        [InlineData("StartName", "TestName")]
        public void UpdateTask_ShouldUpdateTaskAndCategoryFromDatabase(string startName, string newName)
        {
            // Arrange
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();

            context.Categories.Add(_mockCategoryFallbackService.Object.CommonCategory);
            context.Categories.Add(_mockCategoryFallbackService.Object.DeletedCategory);
            context.SaveChanges();
            var taskStore = new TaskStore(context, _mockCategoryFallbackService.Object, _mockLogger.Object);
            var task = new TaskDTO
            {
                Name = startName,
                Status = CasualTaskStatus.Postponed,
                Category = null,
                Description = "",
                DueDate = DateTime.Now,
            };

            var addedTask = taskStore.Add(task);
            context.SaveChanges();

            // Act
            addedTask.Name = newName;
            addedTask.Category = _mockCategoryFallbackService.Object.DeletedCategory;
            var result = taskStore.Update(addedTask);
            context.SaveChanges();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result.Name, addedTask.Name);
            Assert.Equal(result.Name, newName);
            Assert.Equal(result.Category.Name, _mockCategoryFallbackService.Object.DeletedCategory.Name);
            Assert.NotEqual(result.Name, startName);
            Assert.NotEqual(result.Category.Name, _mockCategoryFallbackService.Object.CommonCategory.Name);
        }

        [Theory]
        [InlineData("StartName", "NewName")]
        [InlineData("StartName", "TestName")]
        public void UpdateTask_ShouldUpdateTaskAndNullCategoryFromDatabase(string startName, string newName)
        {
            // Arrange
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();

            context.Categories.Add(_mockCategoryFallbackService.Object.CommonCategory);
            context.Categories.Add(_mockCategoryFallbackService.Object.DeletedCategory);
            context.SaveChanges();
            var taskStore = new TaskStore(context, _mockCategoryFallbackService.Object, _mockLogger.Object);
            var task = new TaskDTO
            {
                Name = startName,
                Status = CasualTaskStatus.Postponed,
                Category = _mockCategoryFallbackService.Object.DeletedCategory,
                Description = "",
                DueDate = DateTime.Now,
            };

            var addedTask = taskStore.Add(task);
            context.SaveChanges();

            // Act
            addedTask.Name = newName;
            addedTask.Category = null;
            var result = taskStore.Update(addedTask);
            context.SaveChanges();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result.Name, addedTask.Name);
            Assert.Equal(result.Name, newName);
            Assert.Equal(result.Category.Name, _mockCategoryFallbackService.Object.DeletedCategory.Name);
            Assert.NotEqual(result.Name, startName);
            Assert.NotEqual(result.Category.Name, _mockCategoryFallbackService.Object.CommonCategory.Name);
        }


        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void UpdateTask_ShouldThrowException(string name)
        {
            // Arrange
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();

            context.Categories.Add(_mockCategoryFallbackService.Object.CommonCategory);
            context.Categories.Add(_mockCategoryFallbackService.Object.DeletedCategory);
            context.SaveChanges();
            var taskStore = new TaskStore(context, _mockCategoryFallbackService.Object, _mockLogger.Object);
            var task = new TaskDTO
            {
                Name = name,
                Status = CasualTaskStatus.Postponed,
                Category = _mockCategoryFallbackService.Object.DeletedCategory,
                Description = "",
                DueDate = DateTime.Now,
            };
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => taskStore.Update(task));
            // Assert
            Assert.Equal("Параметр item=null или item.Name=null", exception.Message);
        }

        [Fact]
        public void UpdateTask_ShouldNullThrowException()
        {
            // Arrange
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();

            context.Categories.Add(_mockCategoryFallbackService.Object.CommonCategory);
            context.Categories.Add(_mockCategoryFallbackService.Object.DeletedCategory);
            context.SaveChanges();
            var taskStore = new TaskStore(context, _mockCategoryFallbackService.Object, _mockLogger.Object);
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => taskStore.Update(null));
            // Assert
            Assert.Equal("Параметр item=null или item.Name=null", exception.Message);
        }


        [Theory]
        [InlineData("StartName", "NewName")]
        [InlineData("StartName", "TestName")]
        public async Task UpdateAsyncTask_ShouldUpdateTaskAndCategoryFromDatabase(string startName, string newName)
        {
            // Arrange
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();

            context.Categories.Add(_mockCategoryFallbackService.Object.CommonCategory);
            context.Categories.Add(_mockCategoryFallbackService.Object.DeletedCategory);
            context.SaveChanges();
            var taskStore = new TaskStore(context, _mockCategoryFallbackService.Object, _mockLogger.Object);
            var task = new TaskDTO
            {
                Name = startName,
                Status = CasualTaskStatus.Postponed,
                Category = null,
                Description = "",
                DueDate = DateTime.Now,
            };

            var addedTask = taskStore.Add(task);
            context.SaveChanges();

            // Act
            addedTask.Name = newName;
            addedTask.Category = _mockCategoryFallbackService.Object.DeletedCategory;
            var result = await taskStore.UpdateAsync(addedTask);
            await context.SaveChangesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result.Name, addedTask.Name);
            Assert.Equal(result.Name, newName);
            Assert.Equal(result.Category.Name, _mockCategoryFallbackService.Object.DeletedCategory.Name);
            Assert.NotEqual(result.Name, startName);
            Assert.NotEqual(result.Category.Name, _mockCategoryFallbackService.Object.CommonCategory.Name);
        }

        [Theory]
        [InlineData("StartName", "NewName")]
        [InlineData("StartName", "TestName")]
        public async Task UpdateAsyncTask_ShouldUpdateTaskAndNullCategoryFromDatabase(string startName, string newName)
        {
            // Arrange
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();

            context.Categories.Add(_mockCategoryFallbackService.Object.CommonCategory);
            context.Categories.Add(_mockCategoryFallbackService.Object.DeletedCategory);
            context.SaveChanges();
            var taskStore = new TaskStore(context, _mockCategoryFallbackService.Object, _mockLogger.Object);
            var task = new TaskDTO
            {
                Name = startName,
                Status = CasualTaskStatus.Postponed,
                Category = _mockCategoryFallbackService.Object.DeletedCategory,
                Description = "",
                DueDate = DateTime.Now,
            };

            var addedTask = taskStore.Add(task);
            context.SaveChanges();

            // Act
            addedTask.Name = newName;
            addedTask.Category = null;
            var result = await taskStore.UpdateAsync(addedTask);
            await context.SaveChangesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result.Name, addedTask.Name);
            Assert.Equal(result.Name, newName);
            Assert.Equal(result.Category.Name, _mockCategoryFallbackService.Object.DeletedCategory.Name);
            Assert.NotEqual(result.Name, startName);
            Assert.NotEqual(result.Category.Name, _mockCategoryFallbackService.Object.CommonCategory.Name);
        }


        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task UpdateAsyncTask_ShouldThrowException(string name)
        {
            // Arrange
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();

            context.Categories.Add(_mockCategoryFallbackService.Object.CommonCategory);
            context.Categories.Add(_mockCategoryFallbackService.Object.DeletedCategory);
            context.SaveChanges();
            var taskStore = new TaskStore(context, _mockCategoryFallbackService.Object, _mockLogger.Object);
            var task = new TaskDTO
            {
                Name = name,
                Status = CasualTaskStatus.Postponed,
                Category = _mockCategoryFallbackService.Object.DeletedCategory,
                Description = "",
                DueDate = DateTime.Now,
            };
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await taskStore.UpdateAsync(task));
            // Assert
            Assert.Equal("Параметр item=null или item.Name=null", exception.Message);
        }

        [Fact]
        public async Task UpdateAsyncTask_ShouldNullThrowException()
        {
            // Arrange
            using var context = new CasualTaskerDbContext(_options);
            context.Database.EnsureCreated();

            context.Categories.Add(_mockCategoryFallbackService.Object.CommonCategory);
            context.Categories.Add(_mockCategoryFallbackService.Object.DeletedCategory);
            context.SaveChanges();
            var taskStore = new TaskStore(context, _mockCategoryFallbackService.Object, _mockLogger.Object);
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await taskStore.UpdateAsync(null));
            // Assert
            Assert.Equal("Параметр item=null или item.Name=null", exception.Message);
        }

        #endregion


    }
}
