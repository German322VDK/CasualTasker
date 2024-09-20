using CasualTasker.Database.StaticData;
using CasualTasker.DTO;
using CasualTasker.Infrastructure.ViewUpdaters;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace CasualTasker.Infrastructure.ObservableDbCollections
{
    public class ObservableTaskDbCollection : ObservableDbCollection<TaskDTO>
    {
        public ObservableTaskDbCollection(ILogger<ObservableDbCollection<TaskDTO>> logger) : base(logger)
        {
            _viewUpdater = new ViewUpdater<TaskDTO>(new ObservableCollection<TaskDTO>(TestData.Tasks));
        }
    }
}
