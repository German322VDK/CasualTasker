using System.Windows.Input;

namespace CasualTasker.Infrastructure.Commands.Base
{
    /// <summary>
    /// Represents an abstract base class that implements the <see cref="ICommand"/> interface.
    /// Provides a simplified way to define command logic by handling <see cref="CanExecuteChanged"/> and
    /// delegating <see cref="ICommand.CanExecute"/> and <see cref="ICommand.Execute"/> to the derived class.
    /// </summary>
    public abstract class Command : ICommand
    {
        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// This event is tied to the <see cref="CommandManager.RequerySuggested"/> event to automatically re-evaluate the command.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// This method is called by the <see cref="ICommand.CanExecute"/> implementation.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <returns><see langword="true"/> if the command can execute; otherwise, <see langword="false"/>.</returns>
        bool ICommand.CanExecute(object parameter) => CanExecute(parameter);

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// This method is called by the <see cref="ICommand.Execute"/> implementation.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        void ICommand.Execute(object parameter) => Execute(parameter);

        /// <summary>
        /// Defines the logic that determines whether the command can be executed.
        /// By default, returns <see langword="true"/>, meaning the command is always executable.
        /// Override this method in derived classes to provide custom logic.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <returns><see langword="true"/> if the command can execute; otherwise, <see langword="false"/>.</returns>
        protected virtual bool CanExecute(object parameter) => true;

        /// <summary>
        /// Defines the logic to be executed when the command is invoked.
        /// This method must be overridden in derived classes to provide the actual command behavior.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        protected abstract void Execute(object parameter);
    }
}
