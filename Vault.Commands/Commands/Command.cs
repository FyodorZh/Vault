using Archivarius;

namespace Vault.Commands
{
    public interface ICommand
    {
        string Name { get; }
        Result Process(IProcessorContext context);
    }

    public abstract class Command : ICommand, IVersionedDataStruct
    {
        public abstract string Name { get; }

        protected static Result Ok => new OkResult();
        protected static Result Fail(string? text = null) => new FailResult(text);
        
        public abstract Result Process(IProcessorContext context);

        public abstract void Serialize(ISerializer serializer);

        public virtual byte Version => 0;
    }
}