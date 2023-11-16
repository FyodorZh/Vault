using OrderedSerializer;

namespace Vault.Scripting
{
    public interface ICommand
    {
        string Name { get; }
        IReadOnlyList<CommandOption> Options { get; }
    }

    public abstract class Command : ICommand, IVersionedDataStruct
    {
        public abstract string Name { get; }

        public virtual IReadOnlyList<CommandOption> Options => Array.Empty<CommandOption>();

        public virtual void Serialize(IOrderedSerializer serializer)
        {
        }

        public virtual byte Version => 0;
    }
}