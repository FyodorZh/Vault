using System.Runtime.InteropServices;

namespace Vault.Scripting
{
    [Guid("C0BF4DFF-2764-4DF7-81E1-1585E469863D")]
    public class AddCommand : Command2
    {
        public override string Name => "add";
        
        private AddCommand()
        {}
        
        public AddCommand(string fileName, string fileContent)
            : base(new CommandOption(fileName), new CommandOption(fileContent))
        {}
    }
}