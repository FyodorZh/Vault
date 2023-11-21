using System;
using System.Collections.Generic;

namespace Vault.Scripting
{
    public interface ICommandSource
    {
        delegate void CommandParseError(Exception? exception);
        
        event CommandParseError? OnError;
        
        IEnumerable<ICommand> GetAll();
    }
}