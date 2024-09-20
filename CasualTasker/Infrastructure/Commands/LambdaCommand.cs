using CasualTasker.Infrastructure.Commands.Base;

namespace CasualTasker.Infrastructure.Commands
{
    public class LambdaCommand : Command
    {
        private readonly Action<object> _Execute;

        private readonly Predicate<object> _CanExecute;

        public LambdaCommand(Action<object> Execute, Predicate<object> CanExecute = null)
        {
            _Execute = Execute ?? throw new ArgumentNullException(nameof(Execute));
            _CanExecute = CanExecute;
        }

        protected override bool CanExecute(object p) => _CanExecute?.Invoke(p) ?? true;

        protected override void Execute(object p) => _Execute(p);
    }
}
