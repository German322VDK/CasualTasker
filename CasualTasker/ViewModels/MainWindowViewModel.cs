using CasualTasker.ViewModels.Base;

namespace CasualTasker.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _title = "CasualTasker";
        public MainWindowViewModel()
        {
            
        }

        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }
    }
}
