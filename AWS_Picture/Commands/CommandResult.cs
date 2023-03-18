using System;

namespace AWS_Picture.Commands
{
    public class CommandResult : CommandBase
    {
        private Action _execute;
        public CommandResult(Action execute)
        {
            _execute = execute;
        }
        public override void Execute(object? parameter)
        {
            _execute.Invoke();
        }
    }
}
