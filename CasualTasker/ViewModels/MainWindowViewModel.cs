using CasualTasker.ViewModels.Base;

namespace CasualTasker.ViewModels
{
    /// <summary>
    /// ViewModel for View MainWindow
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private string _title = "CasualTasker";

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            
        }

        /// <summary>
        /// Gets or sets the title of the main window.
        /// </summary>
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }
    }
}
