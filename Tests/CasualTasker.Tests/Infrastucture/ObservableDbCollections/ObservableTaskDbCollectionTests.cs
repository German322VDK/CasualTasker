using CasualTasker.DTO;
using CasualTasker.Infrastructure.ObservableDbCollections;
using CasualTasker.Services.Fallbacks;
using CasualTasker.Services.Stores;
using Microsoft.Extensions.Logging;
using Moq;

namespace CasualTasker.Tests.Infrastucture.ObservableDbCollections
{
    public class ObservableTaskDbCollectionTests
    {
        private readonly Mock<IStore<TaskDTO>> _storeTaskMock;
        private readonly Mock<ILogger<ObservableTaskDbCollection>> _loggerMock;
        private readonly ObservableTaskDbCollection _observableTaskDbCollection;
        private readonly Mock<ICategoryFallbackService> _mockCategoryFallbackService;

        public ObservableTaskDbCollectionTests()
        {
            _storeTaskMock = new Mock<IStore<TaskDTO>>();
            _loggerMock = new Mock<ILogger<ObservableTaskDbCollection>>();
            _mockCategoryFallbackService = new Mock<ICategoryFallbackService>();
            _mockCategoryFallbackService
                .Setup(x => x.DeletedCategory)
                .Returns(new CategoryDTO
                {
                    Id = 1,
                    Name = "Deleted"
                });
            _mockCategoryFallbackService
                .Setup(x => x.CommonCategory)
                .Returns(new CategoryDTO
                {
                    Id = 2,
                    Name = "Common"
                });

            _observableTaskDbCollection = new ObservableTaskDbCollection(_storeTaskMock.Object, _loggerMock.Object);
        }

        #region Add

        [Theory]
        [InlineData(1, "Test1")]
        [InlineData(0, "Test2")]
        public void Add_ValidTask_AddsToStoreAndUpdatesView(int id, string name)
        {
            // Arrange: подготавливаем данные
            var newCategory = new CategoryDTO { Id = 3, Name = "name" };
            var newTask = new TaskDTO
            {
                Id = id,
                Name = name,
                Description = "",
                DueDate = DateTime.Now,
                Category = newCategory,
                Status = CasualTaskStatus.Postponed,
            };

            // Настраиваем хранилище так, чтобы оно добавляло новый объект и возвращало его
            _storeTaskMock.Setup(store => store.Add(It.Is<TaskDTO>(c => c.Id == id && c.Name == name))).Returns(newTask);
            // Act: вызываем тестируемый метод
            var result = _observableTaskDbCollection.Add(newTask);

            // Assert: проверяем результат и что методы хранилища были вызваны
            Assert.True(result);
            Assert.NotNull(_observableTaskDbCollection.Get(newTask.Id));
            _storeTaskMock.Verify(store => store.Add(It.Is<TaskDTO>(c => c.Id == id && c.Name == name)), Times.Once);
        }

        [Theory]
        [InlineData(1, null)]
        [InlineData(0, "")]
        public void Add_InValidTask_ReturnFalse(int id, string name)
        {
            // Arrange: подготавливаем данные
            var newCategory = new CategoryDTO { Id = 3, Name = "name" };
            var newTask = new TaskDTO
            {
                Id = id,
                Name = name,
                Description = "",
                DueDate = DateTime.Now,
                Category = newCategory,
                Status = CasualTaskStatus.Postponed,
            };

            // Настраиваем хранилище так, чтобы оно добавляло новый объект и возвращало его
            _storeTaskMock.Setup(store => store.Add(It.Is<TaskDTO>(c => c.Id == id && c.Name == name))).Returns(newTask);
            // Act: вызываем тестируемый метод
            var result = _observableTaskDbCollection.Add(newTask);

            // Assert: проверяем результат и что методы хранилища были вызваны
            Assert.False(result);
            Assert.Null(_observableTaskDbCollection.Get(newTask.Id));
            _storeTaskMock.Verify(store => store.Add(It.Is<TaskDTO>(c => c.Id == id && c.Name == name)), Times.Never);
        }

        [Fact]
        public void Add_NullTask_ReturnFalse()
        {
            // Настраиваем хранилище так, чтобы оно добавляло новый объект и возвращало его
            // Act: вызываем тестируемый метод
            var result = _observableTaskDbCollection.Add(null);

            // Assert: проверяем результат и что методы хранилища были вызваны
            Assert.False(result);
        }

        [Theory]
        [InlineData(1, "Test1")]
        [InlineData(0, "Test2")]
        public void Add_ExistingTask_ReturnFalse(int id, string name)
        {
            // Arrange: подготавливаем данные
            var newCategory = new CategoryDTO { Id = 3, Name = "name" };
            var newTask = new TaskDTO
            {
                Id = id,
                Name = name,
                Description = "",
                DueDate = DateTime.Now,
                Category = newCategory,
                Status = CasualTaskStatus.Postponed,
            };

            // Настраиваем хранилище так, чтобы оно добавляло новый объект и возвращало его
            _storeTaskMock.Setup(store => store.Add(It.Is<TaskDTO>(c => c.Id == id && c.Name == name))).Returns(newTask);
            // Act: вызываем тестируемый метод
            _observableTaskDbCollection.Add(newTask);
            _storeTaskMock.Setup(store => store.GetById(It.IsAny<int>())).Returns(newTask);
            var result = _observableTaskDbCollection.Add(newTask);

            // Assert: проверяем результат и что методы хранилища были вызваны
            Assert.False(result);
            Assert.NotNull(_observableTaskDbCollection.Get(newTask.Id));
            _storeTaskMock.Verify(store => store.Add(It.Is<TaskDTO>(c => c.Id == id && c.Name == name)), Times.Once);
        }

        #endregion

        #region Delete

        [Theory]
        [InlineData(1, "Test1")]
        [InlineData(0, "Test2")]
        public void Delete_ValidTask_DeletesToStoreAndUpdatesView(int id, string name)
        {
            // Arrange: подготавливаем данные
            var newCategory = new CategoryDTO { Id = 3, Name = "name" };
            var newTask = new TaskDTO
            {
                Id = id,
                Name = name,
                Description = "",
                DueDate = DateTime.Now,
                Category = newCategory,
                Status = CasualTaskStatus.Postponed,
            };

            // Настраиваем хранилище так, чтобы оно добавляло новый объект и возвращало его
            _storeTaskMock.Setup(store => store.Add(It.Is<TaskDTO>(c => c.Id == id && c.Name == name))).Returns(newTask);
            _storeTaskMock.Setup(store => store.Delete(newTask.Id)).Returns(true);
            // Act: вызываем тестируемый метод
            var addedResult = _observableTaskDbCollection.Add(newTask);
            _storeTaskMock.Setup(store => store.GetById(It.IsAny<int>())).Returns(newTask);
            var deletedResult = _observableTaskDbCollection.Delete(newTask);

            // Assert: проверяем результат и что методы хранилища были вызваны
            Assert.True(addedResult);
            Assert.True(deletedResult);
            Assert.Null(_observableTaskDbCollection.Get(newTask.Id));
            _storeTaskMock.Verify(store => store.Add(It.Is<TaskDTO>(c => c.Id == id && c.Name == name)), Times.Once);
            _storeTaskMock.Verify(store => store.Delete(newTask.Id), Times.Once);
        }

        [Fact]
        public void Delete_NotContainedValidTask_ReturnFalse()
        {
            // Arrange: подготавливаем данные
            var newCategory = new CategoryDTO { Id = 3, Name = "name" };
            var newTask = new TaskDTO
            {
                Id = 1,
                Name = "@",
                Description = "",
                DueDate = DateTime.Now,
                Category = newCategory,
                Status = CasualTaskStatus.Postponed,
            };

            // Act: вызываем тестируемый метод
            var deletedResult = _observableTaskDbCollection.Delete(newTask);

            // Assert: проверяем результат и что методы хранилища были вызваны
            Assert.False(deletedResult);
            Assert.Null(_observableTaskDbCollection.Get(newCategory.Id));
        }

        #endregion

        #region Update

        [Theory]
        [InlineData(2, "Test1", "New1")]
        [InlineData(0, "Test2", "New2")]
        public void Update_ValidTask_UpdatesToStoreAndUpdatesView(int id, string startName, string newName)
        {
            // Arrange: подготавливаем данные
            var newCategory = new CategoryDTO { Id = 6, Name = "startName" };
            var newTask = new TaskDTO
            {
                Id = id,
                Name = startName,
                Description = "",
                DueDate = DateTime.Now,
                Category = newCategory,
                Status = CasualTaskStatus.Postponed,
            };
            var updatedTask = new TaskDTO
            {
                Id = id,
                Name = newName,
                Description = "",
                DueDate = DateTime.Now,
                Category = newCategory,
                Status = CasualTaskStatus.Postponed,
            };
            _storeTaskMock.Setup(store => store.Add(It.IsAny<TaskDTO>())).Returns(newTask);
            _storeTaskMock.Setup(store => store.Update(It.IsAny<TaskDTO>())).Returns(updatedTask);
            var addedResult = _observableTaskDbCollection.Add(newTask);
            _storeTaskMock.Setup(store => store.GetById(It.IsAny<int>())).Returns(newTask);

            // Act: вызываем тестируемый метод
            var updatedResult = _observableTaskDbCollection.Update(updatedTask);
            // Assert: проверяем результат и что методы хранилища были вызваны

            Assert.True(addedResult);
            Assert.True(updatedResult);
            Assert.NotNull(_observableTaskDbCollection.Get(updatedTask.Id));
            Assert.Equal(updatedTask.Name, _observableTaskDbCollection.Get(updatedTask.Id).Name);
        }

        [Theory]
        [InlineData(2, "Test1", null)]
        [InlineData(0, "Test2", "")]
        public void Update_InvalidTask_ReturnsFalse(int id, string startName, string newName)
        {
            // Arrange: подготавливаем данные
            var newCategory = new CategoryDTO { Id = 6, Name = "startName" };
            var newTask = new TaskDTO
            {
                Id = id,
                Name = startName,
                Description = "",
                DueDate = DateTime.Now,
                Category = newCategory,
                Status = CasualTaskStatus.Postponed,
            };
            var updatedTask = new TaskDTO
            {
                Id = id,
                Name = newName,
                Description = "",
                DueDate = DateTime.Now,
                Category = newCategory,
                Status = CasualTaskStatus.Postponed,
            };
            _storeTaskMock.Setup(store => store.Add(It.IsAny<TaskDTO>())).Returns(newTask);
            _storeTaskMock.Setup(store => store.Update(It.IsAny<TaskDTO>())).Returns(updatedTask);
            var addedResult = _observableTaskDbCollection.Add(newTask);
            _storeTaskMock.Setup(store => store.GetById(It.IsAny<int>())).Returns(newTask);

            // Act: вызываем тестируемый метод
            var updatedResult = _observableTaskDbCollection.Update(updatedTask);
            // Assert: проверяем результат и что методы хранилища были вызваны

            Assert.True(addedResult);
            Assert.False(updatedResult);
            Assert.NotNull(_observableTaskDbCollection.Get(updatedTask.Id));
            Assert.Equal(newTask.Name, _observableTaskDbCollection.Get(newTask.Id).Name);
            Assert.NotEqual(updatedTask.Name, _observableTaskDbCollection.Get(updatedTask.Id).Name);
        }

        [Theory]
        [InlineData(2, "Test1")]
        [InlineData(0, "Test2")]
        public void Update_NotExistingCategory_ReturnsFalse(int id, string newName)
        {
            var newCategory = new CategoryDTO { Id = 6, Name = "startName" };
            var newTask = new TaskDTO
            {
                Id = id,
                Name = newName,
                Description = "",
                DueDate = DateTime.Now,
                Category = newCategory,
                Status = CasualTaskStatus.Postponed,
            };

            // Act: вызываем тестируемый метод
            var updatedResult = _observableTaskDbCollection.Update(newTask);
            // Assert: проверяем результат и что методы хранилища были вызваны

            Assert.False(updatedResult);
        }

        #endregion
    }
}
