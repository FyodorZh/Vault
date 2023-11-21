using System.Collections.Generic;
using OrderedSerializer;

namespace Vault.Commands
{
    public abstract class Command1 : Command
    {
        private readonly CommandOption[] _options = new CommandOption[1];

        public sealed override IReadOnlyList<CommandOption> Options => _options;

        public CommandOption Option => _options[0];

        protected Command1()
        {
        }

        protected Command1(CommandOption option)
        {
            _options[0] = option;
        }

        public sealed override void Serialize(IOrderedSerializer serializer)
        {
            serializer.AddStruct(ref _options[0]);
        }
    }
}