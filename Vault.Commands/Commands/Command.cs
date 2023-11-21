using System;
using System.Collections.Generic;
using OrderedSerializer;

namespace Vault.Commands
{
    public interface ICommand
    {
        string Name { get; }
        IReadOnlyList<CommandOption> Options { get; }
        Result Process(IProcessorContext context);
    }

    public abstract class Command : ICommand, IVersionedDataStruct
    {
        public abstract string Name { get; }

        public virtual IReadOnlyList<CommandOption> Options => Array.Empty<CommandOption>();

        protected static Result Ok => new OkResult();
        protected static Result Fail(string? text = null) => new FailResult(text);
        
        public abstract Result Process(IProcessorContext context);

        public virtual void Serialize(IOrderedSerializer serializer)
        {
        }

        public virtual byte Version => 0;
    }
}