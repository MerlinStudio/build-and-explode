using System;

namespace Dev.Core.Ui.Commands
{
    public interface ICommand
    {
        event Action<ICommand, bool> Completed;
        
        void Execute();
    }
}