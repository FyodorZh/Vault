using System.Threading.Tasks;
using Archivarius;

namespace Vault.Commands
{
    public interface ICommand
    {
        string Name { get; }
        Task<Result> Process(IProcessorContext context);
    }

    public abstract class Command : ICommand, IVersionedDataStruct
    {
        public abstract string Name { get; }

        protected static Task<Result> Ok => Task.FromResult<Result>(new OkResult());

        protected static Task<Result> Fail(string? text = null)
        {
            return Task.FromResult<Result>(new FailResult(text));
        }

        public abstract Task<Result> Process(IProcessorContext context);

        public abstract void Serialize(ISerializer serializer);

        public virtual byte Version => 0;
    }
}