using CasualTasker.DTO;
using CasualTasker.Services.Stores;
using Microsoft.Extensions.Logging;

namespace CasualTasker.Infrastructure.ObservableDbCollections
{
    public class ObservableTaskDbCollection : ObservableDbCollection<TaskDTO>
    {
        public ObservableTaskDbCollection(IStore<TaskDTO> store, ILogger<ObservableDbCollection<TaskDTO>> logger)
             : base(store, logger) { }
    }
}
