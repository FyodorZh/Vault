using System.Runtime.InteropServices;

namespace Vault.Scripting
{
    [Guid("B716E624-B7C9-4345-AB92-E85F825BC1FE")]
    public class LockCommand : Command1
    {
        public override string Name => "lock";
        
        private LockCommand() {}
        
        public LockCommand(string scope)
            : base(new CommandOption(scope))
        {}
    }
}