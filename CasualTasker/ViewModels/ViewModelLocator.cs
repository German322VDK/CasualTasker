using Microsoft.Extensions.DependencyInjection;

namespace CasualTasker.ViewModels
{
    /// <summary>
    /// Provides access to ViewModels for data binding in the application.
    /// </summary>
    public sealed class ViewModelLocator
    {
        /// <summary>
        /// Gets the instance of MainWindowViewModel.
        /// </summary>
        public MainWindowViewModel MainWindowVM => App.Services.GetRequiredService<MainWindowViewModel>();

        /// <summary>
        /// Gets the instance of MainPageViewModel.
        /// </summary>
        public MainPageViewModel MainPageVM => App.Services.GetRequiredService<MainPageViewModel>();

        /// <summary>
        /// Gets the instance of EditTaskPageViewModel.
        /// </summary>
        public EditTaskPageViewModel EditTaskVM => App.Services.GetRequiredService<EditTaskPageViewModel>();

        /// <summary>
        /// Gets the instance of EditCategoryPageViewModel.
        /// </summary>
        public EditCategoryPageViewModel EditCategoryVM => App.Services.GetRequiredService<EditCategoryPageViewModel>();
    }
}
