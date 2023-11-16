using System.Runtime.InteropServices;

namespace Vault.Scripting
{
    [Guid("E36473AC-8AA7-41E1-A0F9-EC7D28D0CEC7")]
    public class MkdirCommand : Command1
    {
        public override string Name => "mkdir";
        
        private MkdirCommand() {}
        
        public MkdirCommand(string dirName)
            : base(new CommandOption(dirName))
        {}
    }
}