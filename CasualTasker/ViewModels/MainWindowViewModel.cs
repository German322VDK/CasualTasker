using CasualTasker.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
