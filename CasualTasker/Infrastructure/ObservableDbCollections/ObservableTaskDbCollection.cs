using CasualTasker.DTO;
using CasualTasker.Services.Stores;
using Microsoft.Extensions.Logging;

namespace CasualTasker.Infrastructure.ObservableDbCollections
{
    /// <summary>
    /// Represents a collection of task entities that synchronizes the database with the view.
    /// Inherits functionality from <see cref="ObservableDbCollection{TaskDTO}"/> for managing
    /// tasks in both the database and the user interface.
    /// </summary>
    public class ObservableTaskDbCollection : ObservableDbCollection<TaskDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableTaskDbCollection"/> class.
        /// </summary>
        /// <param name="store">The store that handles task-related operations with the database.</param>
        /// <param name="logger">The logger for logging operations within the task collection.</param>
        public ObservableTaskDbCollection(IStore<TaskDTO> store, ILogger<ObservableDbCollection<TaskDTO>> logger)
             : base(store, logger) { }
    }
}
