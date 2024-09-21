using CasualTasker.DTO;
using CasualTasker.Infrastructure.ObservableDbCollections;
using CasualTasker.Services.Fallbacks;
using CasualTasker.Services.Stores;
using Microsoft.Extensions.Logging;
using Moq;

namespace CasualTasker.Tests.Infrastucture.ObservableDbCollections
{
    public class ObservableCategoryDbCollectionTests
    {
        private readonly Mock<IStore<CategoryDTO>> _storeMock;
        private readonly Mock<ILogger<ObservableCategoryDbCollection>> _loggerMock;
        private readonly ObservableCategoryDbCollection _observableCategoryDbCollection;
        private readonly Mock<ICategoryFallbackService> _mockCategoryFallbackService;

        public ObservableCategoryDbCollectionTests()
        {
            _storeMock = new Mock<IStore<CategoryDTO>>();
            _loggerMock = new Mock<ILogger<ObservableCategoryDbCollection>>();
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

            _observableCategoryDbCollection = new ObservableCategoryDbCollection(_storeMock.Object, _loggerMock.Object, _mockCategoryFallbackService.Object);
        }

        #region Add

        [Theory]
        [InlineData(1, "Test1")]
        [InlineData(0, "Test2")]
        public void Add_ValidCategory_AddsToStoreAndUpdatesView(int id, string name)
        {
            // Arrange: подготавливаем данные
            var newCategory = new CategoryDTO { Id = id, Name = name };

            // Настраиваем хранилище так, чтобы оно добавляло новый объект и возвращало его
            _storeMock.Setup(store => store.Add(It.IsAny<CategoryDTO>())).Returns(newCategory);

            // Act: вызываем тестируемый метод
            var result = _observableCategoryDbCollection.Add(newCategory);

            // Assert: проверяем результат и что методы хранилища были вызваны
            Assert.True(result);
            Assert.NotNull(_observableCategoryDbCollection.Get(newCategory.Id));
            _storeMock.Verify(store => store.Add(It.Is<CategoryDTO>(c => c.Id == id && c.Name == name)), Times.Once);
        }

        [Theory]
        [InlineData(1, null)]
        [InlineData(0, "")]
        public void Add_InValidCategory_ReturnFalse(int id, string name)
        {
            // Arrange: подготавливаем данные
            var newCategory = new CategoryDTO { Id = id, Name = name };
            CategoryDTO returnedCategory = null;
            _storeMock.Setup(store => store.GetById(It.IsAny<int>())).Returns(returnedCategory);
            // Act: вызываем тестируемый метод
            var result = _observableCategoryDbCollection.Add(newCategory);

            // Assert: проверяем результат и что методы хранилища были вызваны
            Assert.False(result);
            Assert.Null(_observableCategoryDbCollection.Get(newCategory.Id));
            _storeMock.Verify(store => store.GetAll(), Times.AtLeastOnce);
        }

        [Fact]
        public void Add_NullCategory_ReturnFalse()
        {
            // Arrange: подготавливаем данные
            CategoryDTO newCategory = null;

            // Act: вызываем тестируемый метод
            var result = _observableCategoryDbCollection.Add(newCategory);

            // Assert: проверяем результат и что методы хранилища были вызваны
            Assert.False(result);
            _storeMock.Verify(store => store.Add(It.IsAny<CategoryDTO>()), Times.Never);
        }

        [Fact]
        public void Add_ContainedCategory_ReturnFalse()
        {
            // Arrange: подготавливаем данные
            var newCategory = new CategoryDTO { Id = 2, Name = "2" };
            _storeMock.Setup(store => store.Add(It.IsAny<CategoryDTO>())).Returns(newCategory);
            // Act: вызываем тестируемый метод
            _observableCategoryDbCollection.Add(newCategory);

            _storeMock.Setup(store => store.GetById(It.IsAny<int>())).Returns(newCategory);

            var result = _observableCategoryDbCollection.Add(newCategory);

            // Assert: проверяем результат и что методы хранилища были вызваны
            Assert.False(result);
            Assert.NotNull(_observableCategoryDbCollection.Get(newCategory.Id));
            _storeMock.Verify(store => store.GetAll(), Times.AtLeastOnce);
        }

        #endregion

        #region Delete

        [Theory]
        [InlineData(2, "Test1")]
        [InlineData(0, "Test2")]
        public void Delete_ValidCategory_DeletesToStoreAndUpdatesView(int id, string name)
        {
            // Arrange: подготавливаем данные
            var newCategory = new CategoryDTO { Id = id, Name = name };

            // Настраиваем хранилище так, чтобы оно добавляло новый объект и возвращало его
            _storeMock.Setup(store => store.Add(It.IsAny<CategoryDTO>())).Returns(newCategory);
            _storeMock.Setup(store => store.Delete(It.IsAny<int>())).Returns(true);

            var addedResult = _observableCategoryDbCollection.Add(newCategory);
            _storeMock.Setup(store => store.GetById(It.IsAny<int>())).Returns(newCategory);
            // Act: вызываем тестируемый метод
            var deletedResult = _observableCategoryDbCollection.Delete(newCategory);

            // Assert: проверяем результат и что методы хранилища были вызваны
            Assert.True(addedResult);
            Assert.True(deletedResult);
            Assert.Null(_observableCategoryDbCollection.Get(newCategory.Id));
        }

        [Fact]
        public void Delete_NotContainedValidCategory_ReturnFalse()
        {
            // Arrange: подготавливаем данные
            var newCategory = new CategoryDTO { Id = 2, Name = "@" };

            // Act: вызываем тестируемый метод
            var deletedResult = _observableCategoryDbCollection.Delete(newCategory);

            // Assert: проверяем результат и что методы хранилища были вызваны
            Assert.False(deletedResult);
            Assert.Null(_observableCategoryDbCollection.Get(newCategory.Id));
        }

        [Fact]
        public void Delete_DeletedCategory_ReturnFalse()
        {
            // Arrange: подготавливаем данные
            var newCategory = _mockCategoryFallbackService.Object.DeletedCategory;
            _storeMock.Setup(store => store.Add(It.IsAny<CategoryDTO>())).Returns(newCategory);
            _storeMock.Setup(store => store.Delete(It.IsAny<int>())).Returns(true);
            var addedResult = _observableCategoryDbCollection.Add(newCategory);
            _storeMock.Setup(store => store.GetById(It.IsAny<int>())).Returns(newCategory);
            // Act: вызываем тестируемый метод
            var deletedResult = _observableCategoryDbCollection.Delete(newCategory);

            // Assert: проверяем результат и что методы хранилища были вызваны
            Assert.False(deletedResult);
        }

        #endregion

        #region Update

        [Theory]
        [InlineData(2, "Test1", "New1")]
        [InlineData(0, "Test2", "New2")]
        public void Update_ValidCategory_UpdatesToStoreAndUpdatesView(int id, string startName, string newName)
        {
            // Arrange: подготавливаем данные
            var newCategory = new CategoryDTO { Id = id, Name = startName };
            var updateCategory = new CategoryDTO { Id = id, Name = newName };
            _storeMock.Setup(store => store.Add(It.IsAny<CategoryDTO>())).Returns(newCategory);
            _storeMock.Setup(store => store.Update(It.IsAny<CategoryDTO>())).Returns(updateCategory);
            var addedResult = _observableCategoryDbCollection.Add(newCategory);
            _storeMock.Setup(store => store.GetById(It.IsAny<int>())).Returns(newCategory);

            // Act: вызываем тестируемый метод
            var updatedResult = _observableCategoryDbCollection.Update(updateCategory);
            // Assert: проверяем результат и что методы хранилища были вызваны

            Assert.True(addedResult);
            Assert.True(updatedResult);
            Assert.NotNull(_observableCategoryDbCollection.Get(updateCategory.Id));
            Assert.Equal(updateCategory.Name, _observableCategoryDbCollection.Get(updateCategory.Id).Name);
        }

        [Theory]
        [InlineData(2, "Test1", null)]
        [InlineData(0, "Test2", "")]
        public void Update_InValidCategory_ReturnFalse(int id, string startName, string newName)
        {
            // Arrange: подготавливаем данные
            var newCategory = new CategoryDTO { Id = id, Name = startName };
            var updateCategory = new CategoryDTO { Id = id, Name = newName };
            _storeMock.Setup(store => store.Add(It.IsAny<CategoryDTO>())).Returns(newCategory);
            var addedResult = _observableCategoryDbCollection.Add(newCategory);
            _storeMock.Setup(store => store.GetById(It.IsAny<int>())).Returns(newCategory);

            // Act: вызываем тестируемый метод
            var updatedResult = _observableCategoryDbCollection.Update(updateCategory);
            // Assert: проверяем результат и что методы хранилища были вызваны

            Assert.True(addedResult);
            Assert.False(updatedResult);
            Assert.NotNull(_observableCategoryDbCollection.Get(updateCategory.Id));
            Assert.NotEqual(updateCategory.Name, _observableCategoryDbCollection.Get(updateCategory.Id).Name);
        }

        [Theory]
        [InlineData(2, "Test1")]
        [InlineData(0, "Test2")]
        public void Update_NotExistingCategory_ReturnsFalse(int id, string newName)
        {
            var updateCategory = new CategoryDTO { Id = id, Name = newName };

            // Act: вызываем тестируемый метод
            var updatedResult = _observableCategoryDbCollection.Update(updateCategory);
            // Assert: проверяем результат и что методы хранилища были вызваны

            Assert.False(updatedResult);
        }

        #endregion
    }
}
