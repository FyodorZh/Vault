using System.Runtime.InteropServices;

namespace Vault.Scripting
{
    [Guid("E81B3238-8B90-4B25-BC6E-2517C5AD7DC9")]
    public class UnlockCommand : Command1
    {
        public override string Name => "unlock";
        
        private UnlockCommand() {}
        
        public UnlockCommand(string scope)
            : base(new CommandOption(scope))
        {}
    }
}