using System;
using System.Collections.Generic;

namespace Vault.Commands
{
    public interface ICommandSource
    {
        delegate void CommandParseError(Exception? exception);
        
        event CommandParseError? OnError;
        
        IEnumerable<ICommand> GetAll();
    }
}