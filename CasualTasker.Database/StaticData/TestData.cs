using CasualTasker.DTO;

namespace CasualTasker.Database.StaticData
{
    public static class TestData
    {
        private static int _taskCount = 10;
        private static int _categoryCount = 10;
        private static int _deltaMinutCount = 10000;
        public static IList<CategoryDTO> Categories => Enumerable.Range(1, _categoryCount)
            .Select(i => new CategoryDTO
            {
                Id = i,
                Name = $"Категория-{i}",
                Color = i % 2 == 0 ? $"#00FF00" : "#FFFF00"
            })
            .ToList();

        public static IList<TaskDTO> Tasks => Enumerable.Range(1, _taskCount)
            .Select(i => new TaskDTO
            {
                Id = i,
                Name = $"Задача-{i}",
                Description = $"Описание-{i}",
                DueDate = DateTime.Now.AddMinutes(new Random().Next(-_deltaMinutCount, _deltaMinutCount)),
                Status = (CasualTaskStatus)new Random().Next(1, 4),
                Category = Categories[new Random().Next(0, Categories.Count)]

            })
            .ToList();
    }
}
