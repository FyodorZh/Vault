using OrderedSerializer;

namespace Vault.Scripting
{
    public abstract class Command2 : Command
    {
        private readonly CommandOption[] _options = new CommandOption[2];

        public sealed override IReadOnlyList<CommandOption> Options => _options;

        protected Command2()
        {
        }

        protected Command2(CommandOption option1, CommandOption option2)
        {
            _options[0] = option1;
            _options[1] = option2;
        }

        public sealed override void Serialize(IOrderedSerializer serializer)
        {
            serializer.AddStruct(ref _options[0]);
            serializer.AddStruct(ref _options[1]);
        }
    }
}