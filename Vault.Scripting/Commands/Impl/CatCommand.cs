using System.Runtime.InteropServices;

namespace Vault.Scripting
{
    [Guid("87DE0D2B-E23A-4DFC-A551-1AF643D02D45")]
    public class CatCommand : Command1
    {
        public override string Name => "cat";
        
        private CatCommand()
        {}

        public CatCommand(string fileName)
            : base(new CommandOption(fileName))
        {
        }
    }
}