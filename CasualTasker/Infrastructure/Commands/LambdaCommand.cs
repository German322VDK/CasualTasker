using CasualTasker.Infrastructure.Commands.Base;

namespace CasualTasker.Infrastructure.Commands
{
    /// <summary>
    /// Represents a command that can be executed via lambda expressions.
    /// Inherits from the <see cref="Command"/> class and allows defining the execute and can-execute logic via delegates.
    /// </summary>
    public class LambdaCommand : Command
    {
        /// <summary>
        /// The delegate to invoke when the command is executed.
        /// </summary>
        private readonly Action<object> _Execute;

        /// <summary>
        /// The delegate to determine whether the command can execute.
        /// </summary>
        private readonly Predicate<object> _CanExecute;

        /// <param name="Execute">The action to execute when the command is invoked. This cannot be <see langword="null"/>.</param>
        /// <param name="CanExecute">The predicate to determine whether the command can execute. If <see langword="null"/>, the command can always execute.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="Execute"/> delegate is <see langword="null"/>.</exception>
        public LambdaCommand(Action<object> Execute, Predicate<object> CanExecute = null)
        {
            _Execute = Execute ?? throw new ArgumentNullException(nameof(Execute));
            _CanExecute = CanExecute;
        }

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="p">The command parameter.</param>
        /// <returns><see langword="true"/> if the command can execute; otherwise, <see langword="false"/>.</returns>
        protected override bool CanExecute(object p) => _CanExecute?.Invoke(p) ?? true;

        /// <summary>
        /// Executes the command logic defined in the delegate passed to the constructor.
        /// </summary>
        /// <param name="p">The command parameter.</param>
        protected override void Execute(object p) => _Execute(p);
    }
}
