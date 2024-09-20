using Microsoft.Extensions.DependencyInjection;

namespace CasualTasker.ViewModels
{
    public class ViewModelLocator
    {
        public MainWindowViewModel MainWindowVM => App.Services.GetRequiredService<MainWindowViewModel>();
        public MainPageViewModel MainPageVM => App.Services.GetRequiredService<MainPageViewModel>();
        public EditTaskPageViewModel EditTaskVM => App.Services.GetRequiredService<EditTaskPageViewModel>();
        public EditCategoryPageViewModel EditCategoryVM => App.Services.GetRequiredService<EditCategoryPageViewModel>();
    }
}
